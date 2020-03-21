using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhilipDaubmeier.ViessmannClient.Model;
using PhilipDaubmeier.ViessmannClient.Model.Auth;
using PhilipDaubmeier.ViessmannClient.Model.Error;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient
{
    public abstract class ViessmannAuthBase : IDisposable
    {
        protected readonly IViessmannConnectionProvider<ViessmannPlatformClient> _connectionProvider;

        private readonly HttpClient _client;
        private readonly HttpClient _authClient;

        private const string _authUri = "https://iam.viessmann.com/idp/v1/authorize";
        private const string _tokenUri = "https://iam.viessmann.com/idp/v1/token";
        private const string _redirectUri = "vicare://oauth-callback/everest";

        private static readonly Semaphore _renewTokenSemaphore = new Semaphore(1, 1);

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public ViessmannAuthBase(IViessmannConnectionProvider<ViessmannPlatformClient> connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _client = connectionProvider.Client;
            _authClient = connectionProvider.AuthClient;
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

            return await _client.SendAsync(request);
        }

        /// <summary>
        /// Calls the given endpoint at the Viessmann api and ensures the request is authenticated
        /// and parses the result afterwards. If no valid access token is present yet, this method
        /// will trigger a new authentication.
        /// </summary>
        protected async Task<TModel> CallViessmannApi<TModel>(Uri uri, Func<TModel, bool>? isSuccess = null)
        {
            var responseString = await (await RequestViessmannApi(uri)).Content.ReadAsStringAsync();

            using var sr = new StringReader(responseString);
            using var jsonTextReader = new JsonTextReader(sr);
            var result = _jsonSerializer.Deserialize<TModel>(jsonTextReader);
            if (result == null || (isSuccess != null && !isSuccess(result)))
                throw ExtractErrorMessage(responseString);

            return result;
        }

        private Exception ExtractErrorMessage(string payload)
        {
            using var sr = new StringReader(payload);
            using var jsonTextReader = new JsonTextReader(sr);
            var errorResponse = _jsonSerializer.Deserialize<ErrorPayload>(jsonTextReader);

            var resetTime = errorResponse?.ExtendedPayload?.LimitReset != null ? (DateTimeOffset?)DateTimeOffset.FromUnixTimeMilliseconds(errorResponse.ExtendedPayload.LimitReset.Value) : null;

            if (errorResponse?.StatusCode?.Equals(429) ?? false)
                throw new HttpRequestException($"Viessmann Platform API rate limit exceeded. {errorResponse?.ExtendedPayload?.Name}: {errorResponse?.ExtendedPayload?.RequestCountLimit}. Will be reset at {resetTime?.UtcDateTime}");

            if (errorResponse?.StatusCode.HasValue ?? false)
                return new IOException($"Viessmann Platform API error code {errorResponse?.StatusCode.Value}. Message: \"{errorResponse?.Message}\"");

            return new IOException("Could not deserialize response.");
        }

        /// <summary>
        /// Ensures that after the completion of this task, a valid non-expired access token is
        /// present in the authentication data object, or throws an exception if unsuccessful.
        /// </summary>
        protected async Task Authenticate()
        {
            if (string.IsNullOrWhiteSpace(_connectionProvider.PlattformInstallationId) || string.IsNullOrWhiteSpace(_connectionProvider.PlattformGatewayId) ||
                string.IsNullOrWhiteSpace(_connectionProvider.PlattformApiClientId) || string.IsNullOrWhiteSpace(_connectionProvider.PlattformApiClientSecret))
                throw new Exception("ViessmannPlatformClient is missing one or more of the mandatory connection provider configuration values.");

            try
            {
                _renewTokenSemaphore.WaitOne();

                if (_connectionProvider.AuthData.IsAccessTokenValid())
                    return;

                await RetrieveAccessToken(await GetAuthorizationCode());

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

        private async Task<string> GetAuthorizationCode()
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_authUri}?type=web_server&client_id={_connectionProvider.PlattformApiClientId}&redirect_uri={_redirectUri}&response_type=code"),
                Method = HttpMethod.Get,
            };

            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_connectionProvider.AuthData.Username}:{_connectionProvider.AuthData.UserPassword}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            var response = await _authClient.SendAsync(request);
            var location = response.Headers.Location.AbsoluteUri;

            var prefix = $"{_redirectUri}?code=";
            if (!location.StartsWith(prefix) || location.Length <= prefix.Length)
                throw new Exception("could not retrieve auth code");

            return location.Substring(prefix.Length);
        }

        private async Task RetrieveAccessToken(string authorizationCode)
        {
            Tuple<string, DateTime> loadedToken = await ParseTokenResponse(await _client.PostAsync(
                            new Uri(_tokenUri), new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                                new KeyValuePair<string, string>("client_id", _connectionProvider.PlattformApiClientId),
                                new KeyValuePair<string, string>("client_secret", _connectionProvider.PlattformApiClientSecret),
                                new KeyValuePair<string, string>("code", authorizationCode),
                                new KeyValuePair<string, string>("redirect_uri", _redirectUri)
                            })));

            await _connectionProvider.AuthData.UpdateTokenAsync(loadedToken.Item1, loadedToken.Item2, string.Empty);
        }

        private async Task<Tuple<string, DateTime>> ParseTokenResponse(HttpResponseMessage response)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();

            using var sr = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(sr);
            var authRaw = _jsonSerializer.Deserialize<AccessTokenResponse>(jsonTextReader);
            if (authRaw?.AccessToken == null || authRaw?.ExpiresIn == null)
                throw new Exception("Could not parse token.");

            return new Tuple<string, DateTime>(authRaw.AccessToken, DateTime.Now.AddSeconds(int.Parse(authRaw.ExpiresIn)));
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}