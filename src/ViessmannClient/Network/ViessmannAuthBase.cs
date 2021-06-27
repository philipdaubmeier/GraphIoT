using PhilipDaubmeier.ViessmannClient.Model;
using PhilipDaubmeier.ViessmannClient.Model.Auth;
using PhilipDaubmeier.ViessmannClient.Model.Error;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient.Network
{
    public abstract class ViessmannAuthBase : IDisposable
    {
        protected readonly IViessmannConnectionProvider<ViessmannPlatformClient> _connectionProvider;

        private readonly HttpClient _client;

        private const string _authUri = "https://iam.viessmann.com/idp/v2/authorize";
        private const string _tokenUri = "https://iam.viessmann.com/idp/v2/token";
        private static List<string> _scopes = new List<string>() { "IoT", "User", "offline_access" };

        private static readonly Semaphore _renewTokenSemaphore = new Semaphore(1, 1);

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ViessmannAuthBase(IViessmannConnectionProvider<ViessmannPlatformClient> connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _client = connectionProvider.Client;

            _jsonSerializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
            _jsonSerializerOptions.Converters.Add(new ScheduleMessageConverter());
        }

        /// <summary>
        /// Returns the uri of the login form that has to be sent back to the users
        /// browser or has to be displayed in an embedded browser view respectively.
        /// </summary>
        public Uri GetLoginUri()
        {
            var queryStringData = new List<(string, string)>()
            {
                ("client_id", _connectionProvider.ClientId),
                ("redirect_uri", _connectionProvider.RedirectUri),
                ("response_type", "code"),
                ("code_challenge", _connectionProvider.CodeChallenge),
                ("code_challenge_method", _connectionProvider.CodeChallengeMethod),
                ("scope", Uri.EscapeUriString(string.Join(' ', _scopes)))
            };
            return new Uri($"{_authUri}?{string.Join('&', queryStringData.Select(x => x.Item1 + "=" + x.Item2))}");
        }

        /// <summary>
        /// After the user filled out the Viessmann login form and was redirected to
        /// the configured callback, this method can be called to complete the login
        /// flow. If this method returns false, an error occured and the user could not
        /// be logged in. If true is returned, the resulting access and refresh tokens
        /// are stored in the connection provider and automatically used in all subsequent
        /// requests. Access tokens are also automatically renewed if expired.
        /// </summary>
        public async Task<bool> TryCompleteLogin(string code)
        {
            try
            {
                await LoadInitialAccessToken(code);
                return _connectionProvider.AuthData.IsAccessTokenValid();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Calls the given endpoint at the Viessmann api and ensures the request is authenticated.
        /// If no valid access token is present yet, this method will trigger a new authentication.
        /// </summary>
        protected async Task<HttpResponseMessage> RequestViessmannApi(Uri uri)
        {
            await Authenticate();

            if (_connectionProvider.AuthData.AccessToken is null)
                throw new IOException("Can not request data without access token.");

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _connectionProvider.AuthData.AccessToken);
            request.Headers.Add("Accept", "application/json");

            return await _client.SendAsync(request);
        }

        /// <summary>
        /// Calls the given endpoint at the Viessmann api and ensures the request is authenticated
        /// and parses the result afterwards. If no valid access token is present yet, this method
        /// will trigger a new authentication.
        /// </summary>
        protected async Task<TData> CallViessmannApi<TWiremessage, TData>(Uri uri)
            where TWiremessage : IWiremessage<TData> where TData : class
        {
            var responseString = await (await RequestViessmannApi(uri)).Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TWiremessage>(responseString, _jsonSerializerOptions);
            if (result?.Data == null)
                throw ExtractErrorMessage(responseString);

            return result.Data;
        }

        private Exception ExtractErrorMessage(string payload)
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorPayload>(payload, _jsonSerializerOptions);

            var resetTime = errorResponse?.ExtendedPayload?.LimitReset != null ? (DateTimeOffset?)DateTimeOffset.FromUnixTimeMilliseconds(errorResponse.ExtendedPayload.LimitReset.Value) : null;

            if (errorResponse?.StatusCode?.Equals(429) ?? false)
                throw new HttpRequestException($"Viessmann Platform API rate limit exceeded. {errorResponse?.ExtendedPayload?.Name}: {errorResponse?.ExtendedPayload?.RequestCountLimit}. Will be reset at {resetTime?.UtcDateTime}");

            if (errorResponse?.StatusCode.HasValue ?? false)
                return new IOException($"Viessmann Platform API error code {errorResponse?.StatusCode}. Message: \"{errorResponse?.Message}\"");

            return new IOException("Could not deserialize response.");
        }

        /// <summary>
        /// Ensures that after the completion of this task, a valid non-expired access token is
        /// present in the authentication data object, or throws an exception if unsuccessful.
        /// </summary>
        protected async Task Authenticate()
        {
            if (string.IsNullOrWhiteSpace(_connectionProvider.ClientId) || string.IsNullOrWhiteSpace(_connectionProvider.RedirectUri))
                throw new Exception("ViessmannPlatformClient is missing one or more of the mandatory connection provider configuration values.");

            try
            {
                _renewTokenSemaphore.WaitOne();

                if (_connectionProvider.AuthData.IsAccessTokenValid())
                    return;

                await RefreshAccessToken();

                // check if we have a valid access token now
                if (_connectionProvider.AuthData.IsAccessTokenValid())
                    return;

                throw new IOException("Could not authenticate");
            }
            finally
            {
                _renewTokenSemaphore.Release();
            }
        }

        public async Task LoadInitialAccessToken(string code)
        {
            var formData = new List<(string, string)>()
            {
                ("client_id", _connectionProvider.ClientId),
                ("redirect_uri", _connectionProvider.RedirectUri),
                ("grant_type", "authorization_code"),
                ("code_verifier", _connectionProvider.CodeVerifier),
                ("code", code)
            };
            var loadedToken = await ParseTokenResponse(await _client.PostAsync(new Uri(_tokenUri),
                new FormUrlEncodedContent(formData.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))));

            await _connectionProvider.AuthData.UpdateTokenAsync(loadedToken.Item1, loadedToken.Item2, loadedToken.Item3);
        }

        private async Task RefreshAccessToken()
        {
            if (_connectionProvider.AuthData.RefreshToken is null)
                throw new IOException("No refresh token present");

            var formData = new List<(string, string)>()
            {
                ("client_id", _connectionProvider.ClientId),
                ("grant_type", "refresh_token"),
                ("refresh_token", _connectionProvider.AuthData.RefreshToken)
            };
            var loadedToken = await ParseTokenResponse(await _client.PostAsync(new Uri(_tokenUri),
                new FormUrlEncodedContent(formData.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))));

            await _connectionProvider.AuthData.UpdateTokenAsync(loadedToken.Item1, loadedToken.Item2, loadedToken.Item3);
        }

        private async Task<(string, DateTime, string)> ParseTokenResponse(HttpResponseMessage response)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();

            var authRaw = await JsonSerializer.DeserializeAsync<AccessTokenResponse>(responseStream, _jsonSerializerOptions);
            if (authRaw?.AccessToken == null || authRaw?.ExpiresIn == null)
                throw new Exception("Could not parse token.");

            if (authRaw?.RefreshToken == null)
                throw new Exception("Did not get refresh token.");

            return (authRaw.AccessToken, DateTime.Now.AddSeconds(authRaw.ExpiresIn), authRaw.RefreshToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}