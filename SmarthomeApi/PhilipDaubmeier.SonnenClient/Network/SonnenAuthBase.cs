using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using PhilipDaubmeier.SonnenClient.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SonnenClient.Network
{
    public abstract class SonnenAuthBase
    {
        private const string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:66.0) Gecko/20100101 Firefox/66.0";

        private const string _authBaseUri = @"https://account.sonnen.de/";
        private const string _redirectUri = @"https://my.sonnen.de/";

        private static readonly Semaphore _renewTokenSemaphore = new Semaphore(1, 1);

        private readonly ISonnenConnectionProvider _provider;
        private readonly ISonnenAuth _authData;

        private static readonly CookieContainer _cookieContainer = new CookieContainer();
        private static HttpMessageHandler _clientHandler;
        private static HttpClient _client;

        private static readonly IContractResolver _jsonResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer() { ContractResolver = _jsonResolver };

        private class AuthState
        {
            public string Nonce { get; }
            public string CodeVerifier { get; }
            public string CodeChallenge { get; }
            public string CodeChallengeMethod { get; }
            public string State { get; }

            public string AuthenticityToken { get; set; }
            public string AuthorizationCode { get; set; }

            public AuthState(string stateUrl)
            {
                var random = new AuthRandom();
                Nonce = random.GenerateNonce();
                CodeVerifier = random.GenerateCodeVerifier();
                CodeChallenge = CodeVerifier.ToSHA256Base64UrlSafe();
                CodeChallengeMethod = "S256";
                State = JsonConvert.SerializeObject(new { s = new { url = stateUrl }, v = CodeVerifier }).ToBase64UrlSafe();
            }
        }

        public SonnenAuthBase(ISonnenConnectionProvider connectionProvider)
        {
            _provider = connectionProvider;
            _authData = _provider?.AuthData;
            _clientHandler = _provider?.Handler ?? new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = _cookieContainer
            };
            _client = new HttpClient(_clientHandler);
        }

        /// <summary>
        /// Calls the given endpoint at my-api.sonnen.de and ensures the request is authenticated.
        /// If no valid access token is present yet, this method will trigger a fresh sign-in or refresh
        /// the token with a refresh token if present. The access token will automatically be added
        /// as a Bearer in the Authentication header, together with other common headers.
        /// </summary>
        protected async Task<HttpResponseMessage> RequestSonnenApi(Uri uri)
        {
            await Authenticate();

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Accept", "application/json,application/vnd.api+json");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Origin", "https://my.sonnen.de");
            request.Headers.Host = "my-api.sonnen.de";
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_authData.AccessToken}");
            request.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
            return await _client.SendAsync(request);
        }

        /// <summary>
        /// Calls the given endpoint at my-api.sonnen.de and ensures the request is authenticated
        /// and parses the result afterwards, including unpacking of the wiremessage.
        /// If no valid access token is present yet, this method will trigger a fresh sign-in
        /// or refresh the token with a refresh token if present and append it to the request.
        /// </summary>
        protected async Task<TData> CallSonnenApi<TWiremessage, TData>(Uri uri)
            where TWiremessage : IWiremessage<TData> where TData : class
        {
            var responseStream = await (await RequestSonnenApi(uri)).Content.ReadAsStreamAsync();

            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
                return _jsonSerializer.Deserialize<TWiremessage>(jsonTextReader)?.ContainedData;
        }

        /// <summary>
        /// Ensures that after the completion of this task, a valid non-expired access token is
        /// present in the authentication data object, or throws an exception if unsuccessful.
        /// </summary>
        protected async Task Authenticate()
        {
            try
            {
                _renewTokenSemaphore.WaitOne();

                // if we already have a valid access token, we are done
                if (_authData.IsAccessTokenValid())
                    return;

                // if we have a refresh token, try to fetch a new access token with it
                if (!string.IsNullOrWhiteSpace(_authData.RefreshToken))
                {
                    await RequestAccessTokenWithRefreshToken();

                    // check if we have a valid access token now, if not proceed to new sign-in
                    if (_authData.IsAccessTokenValid())
                        return;
                }

                // sign-in flow: (1) make an authenticate call with a generated challenge,
                // which returns with an html page with the login form and an authenticity
                // token of the server. (2) submit the user credentials and the authenticity
                // token and get back a authorization code (3) request a OAuth access token
                // with the challenge from step 1 and the authorization code from step 2.
                var state = await RequestAuthenticityToken();
                state = await RequestAuthorizationCode(state);
                await RequestAccessTokenWithAuthCode(state);

                // check if we have a valid access token now
                if (_authData.IsAccessTokenValid())
                    return;

                throw new IOException("Could not authenticate");
            }
            finally
            {
                _renewTokenSemaphore.Release();
            }
        }

        private void AddCommonAuthHeaders(HttpRequestHeaders headers, string referrer)
        {
            headers.Add("Accept-Language", "en-US,en;q=0.5");
            headers.Add("Connection", "keep-alive");
            headers.Add("Pragma", "no-cache");
            headers.Add("Cache-Control", "no-cache");
            headers.Host = "account.sonnen.de";
            if (!string.IsNullOrWhiteSpace(referrer))
                headers.Referrer = new Uri(referrer);
            headers.TryAddWithoutValidation("User-Agent", _userAgent);
        }

        private async Task<AuthState> RequestAuthenticityToken()
        {
            var state = new AuthState("/landing");

            var authorizeParameters = new List<Tuple<string, string>>() {
                new Tuple<string, string>("client_id", _provider.ClientId),
                new Tuple<string, string>("response_type", "code"),
                new Tuple<string, string>("redirect_uri", _redirectUri),
                new Tuple<string, string>("code_challenge", state.CodeChallenge),
                new Tuple<string, string>("nonce", state.Nonce),
                new Tuple<string, string>("code_challenge_method", state.CodeChallengeMethod),
                new Tuple<string, string>("state", state.State),
                new Tuple<string, string>("locale", "de")
            };

            var queryString = string.Join('&', authorizeParameters.Select(x => $"{x.Item1}={x.Item2}"));
            var requestUri = new Uri($"{_authBaseUri}oauth/authorize?{queryString}");
            return await ParseAuthenticityToken(await _client.GetAsync(requestUri), state);
        }

        private async Task<AuthState> ParseAuthenticityToken(HttpResponseMessage response, AuthState state)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new IOException("Authorize call did not return with a success HTTP status code, see inner exception.", ex);
            }

            var htmlLoginPage = await response.Content.ReadAsStringAsync();
            string authenticityToken = htmlLoginPage.ReadHiddenHtmlInputValue("authenticity_token");
            if (string.IsNullOrEmpty(authenticityToken))
                throw new IOException("Could not read authenticity token from html page.");

            state.AuthenticityToken = authenticityToken;
            return state;
        }

        private async Task<AuthState> RequestAuthorizationCode(AuthState state)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_authBaseUri}users/sign_in"));
            AddCommonAuthHeaders(request.Headers, $"{_authBaseUri}users/sign_in");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("authenticity_token", state.AuthenticityToken),
                    new KeyValuePair<string, string>("user[email]", _provider.AuthData.Username),
                    new KeyValuePair<string, string>("user[password]", _provider.AuthData.UserPassword),
                    new KeyValuePair<string, string>("user[remember_me]", "0"),
                    new KeyValuePair<string, string>("commit", "Log+in")
                });

            return ParseAuthorizationCode(await _client.SendAsync(request), state);
        }

        private AuthState ParseAuthorizationCode(HttpResponseMessage response, AuthState state)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new IOException("Sign in call did not return with a success HTTP status code, see inner exception.", ex);
            }

            var responseUri = response.RequestMessage.RequestUri;
            var querystring = responseUri.ParseQueryString();
            if (!querystring.Keys.Cast<string>().Contains("code"))
                throw new IOException("Response redirect uri does not contain the authorization code in its querystring.");

            var code = querystring["code"];
            if (string.IsNullOrWhiteSpace(code))
                throw new IOException("Response redirect uri does not contain a valid authorization code.");

            state.AuthorizationCode = code;
            return state;
        }

        private async Task RequestAccessTokenWithAuthCode(AuthState state)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_authBaseUri}oauth/token"));
            AddCommonAuthHeaders(request.Headers, string.Empty);
            request.Headers.Add("Accept", "application/json,application/vnd.api+json");
            request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", state.AuthorizationCode),
                    new KeyValuePair<string, string>("client_id", _provider.ClientId),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                    new KeyValuePair<string, string>("code_verifier", state.CodeVerifier)
                });

            await ParseAccessTokenResponse(await _client.SendAsync(request));
        }

        private async Task RequestAccessTokenWithRefreshToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_authBaseUri}oauth/token"));
            AddCommonAuthHeaders(request.Headers, string.Empty);
            request.Headers.Add("Accept", "application/json,application/vnd.api+json");
            request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", _provider.ClientId),
                    new KeyValuePair<string, string>("refresh_token", _authData.RefreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token")
                });

            await ParseAccessTokenResponse(await _client.SendAsync(request));
        }

        private async Task ParseAccessTokenResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var definition = new
            {
                access_token = "",
                token_type = "",
                expires_in = 0,
                refresh_token = "",
                created_at = 0
            };

            try
            {
                var tokenResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);

                var expiry = Instant.FromUnixTimeSeconds(tokenResponse.created_at).ToDateTimeUtc().ToLocalTime().AddSeconds(tokenResponse.expires_in);
                await _provider.AuthData.UpdateTokenAsync(tokenResponse.access_token, expiry, tokenResponse.refresh_token);
            }
            catch (Exception ex)
            {
                throw new IOException("Unexpected format of access token response, see inner exception.", ex);
            }
        }
    }
}