using PhilipDaubmeier.WeConnectClient.Model;
using PhilipDaubmeier.WeConnectClient.Model.Auth;
using PhilipDaubmeier.WeConnectClient.Model.Core;
using PhilipDaubmeier.WeConnectClient.Model.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    /// <remarks>
    /// Was rebuilt from scratch in October 2021 after the WeConnect portal and its login flow
    /// was changed completely a few months before.
    ///
    /// Was previously (now not anymore) built based on the excellent work of github users bgewehr,
    /// wez3 and reneboer and the volkswagen carnet python client, see here:
    /// https://github.com/bgewehr/volkswagen-carnet-client
    /// </remarks>
    public abstract class WeConnectAuthBase
    {
        protected readonly IWeConnectConnectionProvider _connectionProvider;

        private const string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:75.0) Gecko/20100101 Firefox/75.0";

        private protected readonly AuthState _state = new();

        private readonly HttpClient _client;
        private readonly HttpClient _authClient;

        private const string _landingPage = "https://www.volkswagen.de/";
        private const string _authProxyUri = "https://www.volkswagen.de/app/authproxy";
        private const string _tokenExchangeUri = "https://myvw-idk-token-exchanger.apps.emea.vwapps.io/token-exchange";
        private const string _idpUri = "https://identity.vwgroup.io";

        private const string _fagVw = "vw-de";
        private const string _fagWeConnect = "vwag-weconnect";

        private readonly List<FeatureAppGroup> _featureAppGroups = new()
        {
            new() { Name = _fagVw, Scopes = new(){ "profile", "address", "phone", "dealers", "carConfigurations", "cars", "vin", "profession" }, Prompt = "login" },
            new() { Name = _fagWeConnect, Scopes = new(){ "openid", "mbb" }, Prompt = "none" }
        };

        private static readonly Semaphore _renewTokenSemaphore = new(1, 1);

        private class FeatureAppGroup
        {
            public string Name { get; set; } = null!;
            public List<string> Scopes { get; set; } = null!;
            public string Prompt { get; set; } = null!;

            public static string ToQuerystring(IEnumerable<FeatureAppGroup> fagList)
            {
                var fag = new[]{ $"fag={string.Join(',', fagList.Select(x => x.Name))}" };
                var scopes = fagList.Select(x => $"scope-{x.Name}={string.Join(',', x.Scopes)}");
                var prompts = fagList.Select(x => $"prompt-{x.Name}={x.Prompt}");
                return string.Join('&', fag.Concat(scopes).Concat(prompts));
            }
        }

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public WeConnectAuthBase(IWeConnectConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _client = connectionProvider.Client;
            _authClient = connectionProvider.AuthClient;
        }

        /// <summary>
        /// Calls the given endpoint at the WeConnect api and ensures the request is authenticated
        /// and parses the result afterwards, including unpacking of the wiremessage. If the first request
        /// fails, it tries to reauthenticate and tries it again before throwing the exception to the caller.
        /// </summary>
        private protected async Task<TData> CallApi<TWiremessage, TData>(Uri uri, bool proxyToken = false)
            where TWiremessage : class, IWiremessage<TData> where TData : class
        {
            TData? result;
            try
            {
                result = await CallApiNoRetry<TWiremessage, TData>(uri, proxyToken);
            }
            catch
            {
                // retry login
                await _connectionProvider.AuthData.UpdateTokenAsync(null, DateTime.MinValue, null);

                try
                {
                    result = await CallApiNoRetry<TWiremessage, TData>(uri, proxyToken);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Login retry unsucessful, API response could not be parsed, see inner exception.", ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Calls the given endpoint at the WeConnect api and ensures the request is authenticated
        /// and parses the result afterwards, including unpacking of the wiremessage.
        /// </summary>
        private protected async Task<TData> CallApiNoRetry<TWiremessage, TData>(Uri uri, bool proxyToken = false)
            where TWiremessage : class, IWiremessage<TData> where TData : class
        {
            var response = await RequestApi(uri, proxyToken);
            if (!response.IsSuccessStatusCode)
                throw new IOException($"The API responded with HTTP status code: {(int)response.StatusCode} {response.StatusCode}");

            var responseStream = await response.Content.ReadAsStreamAsync();
            var responseJson = await JsonSerializer.DeserializeAsync<TWiremessage>(responseStream, _jsonSerializerOptions);

            if (responseJson != null && responseJson.HasError)
                throw new IOException($"The API response returned the error code: {responseJson.ErrorCode}");

            if (responseJson?.Body == null)
                throw new IOException($"No valid response payload received");

            return responseJson.Body;
        }

        /// <summary>
        /// Calls the given endpoint at the WeConnect api and ensures the request is authenticated.
        /// </summary>
        protected async Task<HttpResponseMessage> RequestApi(Uri uri, bool proxyToken = false)
        {
            await Authenticate(_state);

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var token = proxyToken ? (string)_state.AuthProxyVwAccessToken : _connectionProvider.AuthData.AccessToken;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("User-ID", _state.UserId);
            request.Headers.Add("traceId", Guid.NewGuid().ToString("D"));

            return await _client.SendAsync(request);
        }

        /// <summary>
        /// Ensures that after the completion of this task, a valid base json uri and
        /// a session cookie is present to be able to query the portal json apis.
        /// </summary>
        private protected async Task Authenticate(AuthState state)
        {
            try
            {
                _renewTokenSemaphore.WaitOne();

                if (_connectionProvider.AuthData.IsAccessTokenValid())
                    return;

                await GetLoginHmacToken(state);
                await LoginIdentifier(state);
                await LoginAuthenticate(state);
                _state.AuthProxyVwAccessToken = await GetAuthProxyAccessToken(state, _fagVw);
                _state.AuthProxyWeConnectAccessToken = await GetAuthProxyAccessToken(state, _fagWeConnect);
                await GetUserId(state);
                await ExchangeAccessToken(state);

                if (!_connectionProvider.AuthData.IsAccessTokenValid())
                    throw new IOException("Login flow completed, still no access token present.");
            }
            catch (Exception innerEx)
            {
                throw new IOException("Could not authenticate, see inner exception for details", innerEx);
            }
            finally
            {
                _renewTokenSemaphore.Release();
            }
        }

        private static void AddCommonAuthHeaders(HttpRequestHeaders headers, string? csrf = null)
        {
            headers.Add("Accept-Language", "en-US,en;q=0.5");
            headers.Add("Accept", "text/html,application/xhtml+xml,application/xml,application/json;q=0.9,*/*;q=0.8");
            headers.Add("Connection", "keep-alive");
            headers.Add("Pragma", "no-cache");
            headers.Add("Cache-Control", "no-cache");
            if (csrf is not null)
                headers.Add("X-CSRF-TOKEN", csrf);
            headers.TryAddWithoutValidation("User-Agent", _userAgent);
        }

        /// <summary>
        /// Step 1
        /// Load login page, and store client id, relay state, hmac token 1 and login CSRF
        /// https://www.volkswagen.de/app/authproxy/login?fag=<featureappgroups>&scope-<fag>=<scopes>&prompt-<fag>=login&redirectUrl=<uri>
        /// </summary>
        private async Task GetLoginHmacToken(AuthState state)
        {
            var loginUri = $"{_authProxyUri}/login?{FeatureAppGroup.ToQuerystring(_featureAppGroups)}&redirectUrl={_landingPage}";
            var loginFormLocationRequest = new HttpRequestMessage(HttpMethod.Get, loginUri);
            AddCommonAuthHeaders(loginFormLocationRequest.Headers);
            var loginFormLocationResponse = await _authClient.SendAsync(loginFormLocationRequest);

            // We get a SESSION set-cookie here!
            // performs a 302 redirect to https://identity.vwgroup.io/oidc/v1/authorize?response_type=code&client_id=<client-id>&scope=<scopes>&state=<state>&redirect_uri=<uri>&prompt=login
            // then another 302 redirect to https://identity.vwgroup.io/signin-service/v1/signin/<client-id>?relayState=<relay-state>
            // read the client id and relay state from these redirect urls
            while (loginFormLocationResponse.StatusCode == HttpStatusCode.Found)
            {
                Uri? redirectLocation = loginFormLocationResponse.Headers.Location ?? throw new IOException("Failed to get sign-in page, no redirect url given.");

                if (redirectLocation.TryExtractUriParameter("client_id", out string clientId))
                    state.ClientId = clientId;

                if (redirectLocation.TryExtractUriParameter("relayState", out string loginRelayStateToken))
                    state.RelayStateToken = loginRelayStateToken;

                loginFormLocationResponse = await _authClient.GetAsync(redirectLocation);
            }

            if (!loginFormLocationResponse.IsSuccessStatusCode)
                throw new IOException($"Failed to get sign-in page, status code {(int)loginFormLocationResponse.StatusCode}.");

            if (string.IsNullOrWhiteSpace(state.ClientId))
                throw new IOException("Failed to get relay state.");

            if (string.IsNullOrWhiteSpace(state.RelayStateToken))
                throw new IOException("Failed to get client_id.");

            // Get hmac and csrf tokens from form content.
            var loginFormPageBody = (await loginFormLocationResponse.Content.ReadAsStringAsync()).Replace("\n", "").Replace("\r", "");
            if (!loginFormPageBody.TryExtractLoginHmac(out string hmac))
                throw new IOException("Failed to get 1st HMAC token.");
            if (!loginFormPageBody.TryExtractLoginCsrf(out string csrf))
                throw new IOException("Failed to get login CSRF.");

            state.HmacToken1 = hmac;
            state.LoginCsrf = csrf;
        }

        /// <summary>
        /// Step 2
        /// Submit username (i.e. email) to first login page
        /// https://identity.vwgroup.io/signin-service/v1/<client_id>/login/identifier
        /// </summary>
        private async Task LoginIdentifier(AuthState state)
        {
            var loginActionUri = new Uri($"{_idpUri}/signin-service/v1/{state.ClientId}/login/identifier");
            var loginActionUrlRequest = new HttpRequestMessage(HttpMethod.Post, loginActionUri);
            AddCommonAuthHeaders(loginActionUrlRequest.Headers);
            loginActionUrlRequest.FormUrlEncoded(new[]
            {
                ("email", _connectionProvider.AuthData.Username),
                ("relayState", state.RelayStateToken),
                ("hmac", state.HmacToken1),
                ("_csrf", state.LoginCsrf)
            });
            var loginActionUrlResponse = await _client.SendAsync(loginActionUrlRequest);

            // performs a 303 redirect to https://identity.vwgroup.io/signin-service/v1/<client_id>/login/authenticate?relayState=<relayState>&email=<email>
            // redirected GET returns form used below.
            if (!loginActionUrlResponse.IsSuccessStatusCode)
                throw new IOException("Failed to get login/identifier page.");

            var loginActionBody = (await loginActionUrlResponse.Content.ReadAsStringAsync()).Replace("\n", "").Replace("\r", "");
            if (!loginActionBody.TryExtractLoginHmac(out string hmac))
                throw new IOException("Failed to get 2nd HMAC token.");

            state.HmacToken2 = hmac;
        }

        /// <summary>
        /// Step 3
        /// Submit password to second login page
        /// https://identity.vwgroup.io/signin-service/v1/<client_id>/login/authenticate
        /// </summary>
        private async Task LoginAuthenticate(AuthState state)
        {
            var loginAction2Uri = new Uri($"{_idpUri}/signin-service/v1/{state.ClientId}/login/authenticate");
            var loginAction2UrlRequest = new HttpRequestMessage(HttpMethod.Post, loginAction2Uri);
            AddCommonAuthHeaders(loginAction2UrlRequest.Headers);
            loginAction2UrlRequest.FormUrlEncoded(new[]
            {
                ("email", _connectionProvider.AuthData.Username),
                ("password", _connectionProvider.AuthData.UserPassword),
                ("relayState", state.RelayStateToken),
                ("hmac", state.HmacToken2),
                ("_csrf", state.LoginCsrf),
                ("login", "true")
            });
            var loginAction2UrlResponse = await _client.SendAsync(loginAction2UrlRequest);

            // (1) the login form flow to retrieve the authorization code for feature app group "vw-de":
            // performs a 302 redirect to https://identity.vwgroup.io/oidc/v1/oauth/sso?clientId=<client_id>&relayState=<relay_state>&userId=<userID>&HMAC=<hmac>
            // then a 302 redirect to GET https://identity.vwgroup.io/signin-service/v1/consent/users/<userID>/<client_id>?scopes=<scopes>&relayState=<relay_state>&callback=https://identity.vwgroup.io/oidc/v1/oauth/client/callback&hmac=<hmac>
            // then a 302 redirect to GET https://identity.vwgroup.io/oidc/v1/oauth/client/callback/success?user_id=<userID>&client_id=<client_id>&scopes=<scopes>&consentedScopes=<scopes>&relayState=<relay_state>&hmac=<hmac>
            // then a 302 redirect to GET https://www.volkswagen.de/app/authproxy/login/oauth2/code/vw-de?state=<state>&code=<authorization_code>
            // (2) followed by a non-prompting flow to retrieve the authorization code for feature app group "vwag-weconnect":
            // then a 302 redirect to GET https://www.volkswagen.de/app/authproxy/login/vwag-weconnect?scope=openid,mbb&prompt=none
            // then a 302 redirect to GET https://identity.vwgroup.io/oidc/v1/authorize?response_type=code&client_id=<client_id>&scope=openid%20mbb&state=<state>&redirect_uri=https://www.volkswagen.de/app/authproxy/login/oauth2/code/vwag-weconnect&nonce=<nonce>&prompt=none
            // then a 302 redirect to GET https://identity.vwgroup.io/oidc/v1/oauth/sso?clientId=<client_id>&userId=<userID>&relayState=<relay_state>&prompt=none&HMAC=<hmac>
            // then a 302 redirect to GET https://identity.vwgroup.io/oidc/v1/oauth/client/callback?clientId=<client_id>&relayState=<relay_state>&userId=<userID>&HMAC=<hmac>
            // then a 302 redirect to GET https://www.volkswagen.de/app/authproxy/login/oauth2/code/vwag-weconnect?state=<state>&code=<authorization_code>

            if (!loginAction2UrlResponse.IsSuccessStatusCode)
                throw new IOException("Failed to process login sequence.");

            // finally read csrf token from cookie
            var cookies = _connectionProvider.CookieContainer?.GetCookies(new Uri(_landingPage)).Cast<Cookie>()
                .GroupBy(c => c.Name.ToUpperInvariant()).ToDictionary(c => c.Key, c => c.First().Value);

            if (cookies is null || !cookies.TryGetValue("CSRF_TOKEN", out string? csrf))
                throw new IOException("Failed to read CSRF token from cookies.");

            state.Csrf = csrf;
        }

        /// <summary>
        /// Step 4
        /// Get access token from auth proxy
        /// </summary>
        private async Task<string> GetAuthProxyAccessToken(AuthState state, string fag)
        {
            var tokenUri = new Uri($"{_authProxyUri}/{fag}/tokens");
            var tokenRequest = new HttpRequestMessage(HttpMethod.Get, tokenUri);
            AddCommonAuthHeaders(tokenRequest.Headers, state.Csrf);
            var tokenResponse = await _client.SendAsync(tokenRequest);

            if (!tokenResponse.IsSuccessStatusCode)
                throw new IOException($"Failed to fetch tokens of feature app group '{fag}'.");

            var tokens = await JsonSerializer.DeserializeAsync<AuthProxyTokenResponse>(await tokenResponse.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

            if (tokens?.AccessToken == null)
                throw new IOException($"No access token present in response for feature app group '{fag}'");

            return tokens.AccessToken;
        }

        /// <summary>
        /// Step 5
        /// Get access token from auth proxy
        /// </summary>
        private async Task GetUserId(AuthState state)
        {
            var userIdUri = new Uri($"{_authProxyUri}/{_fagVw}/user");
            var userIdRequest = new HttpRequestMessage(HttpMethod.Get, userIdUri);
            AddCommonAuthHeaders(userIdRequest.Headers, state.Csrf);
            var userIdResponse = await _client.SendAsync(userIdRequest);

            if (!userIdResponse.IsSuccessStatusCode)
                throw new IOException("Failed to fetch user id.");

            var user = await JsonSerializer.DeserializeAsync<UserProfileResponse>(await userIdResponse.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

            if (string.IsNullOrEmpty(user?.Id))
                throw new IOException("No user id returned.");

            state.UserId = user.Id;
        }

        /// <summary>
        /// Step 6
        /// Exchange auth proxy access token to get an access token for WeConnect access
        /// </summary>
        private async Task ExchangeAccessToken(AuthState state)
        {
            var tokenExchangeUri = new Uri($"{_tokenExchangeUri}?isWcar=false");
            var tokenExchangeRequest = new HttpRequestMessage(HttpMethod.Get, tokenExchangeUri);
            tokenExchangeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", state.AuthProxyWeConnectAccessToken);
            tokenExchangeRequest.Headers.Add("Accept", "application/json, text/plain, */*");
            var tokenExchangeResponse = await _client.SendAsync(tokenExchangeRequest);

            if (!tokenExchangeResponse.IsSuccessStatusCode)
                throw new IOException("Failed to exchange token.");

            var token = await tokenExchangeResponse.Content.ReadAsStringAsync() ?? throw new IOException("Token could not be exchanged.");

            state.AccessToken = token;

            await _connectionProvider.AuthData.UpdateTokenAsync(state.AccessToken, state.AccessToken.Expiration.ToLocalTime(), null);
        }
    }
}