using PhilipDaubmeier.NetatmoClient.Model;
using PhilipDaubmeier.NetatmoClient.Model.Auth;
using PhilipDaubmeier.NetatmoClient.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoClient
{
    public abstract class NetatmoAuthBase : IDisposable
    {
        private const string _authUri = "https://api.netatmo.com/oauth2/authorize";
        protected const string _baseUri = @"https://api.netatmo.net";
        private const string _state = "HwOdqNKAWeKl7";

        private static readonly Semaphore _renewTokenSemaphore = new(1, 1);

        private readonly INetatmoConnectionProvider _provider;
        private readonly INetatmoAuth _authData;

        private readonly HttpClient _client;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };

        public class SnakeCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public NetatmoAuthBase(INetatmoConnectionProvider connectionProvider)
        {
            _provider = connectionProvider;
            _authData = _provider.AuthData;
            _client = _provider.Client;

            _jsonSerializerOptions.Converters.Add(new MeasureConverter());
        }

        /// <summary>
        /// Returns the uri of the login form that has to be sent back to the users
        /// browser or has to be displayed in an embedded browser view respectively.
        /// </summary>
        public Uri GetLoginUri()
        {
            var queryStringData = new List<(string, string)>()
            {
                ("client_id", _provider.AppId),
                ("redirect_uri", _provider.RedirectUri),
                ("scope", Uri.EscapeDataString(_provider.Scope)),
                ("state", _state)
            };
            return new Uri($"{_authUri}?{string.Join('&', queryStringData.Select(x => x.Item1 + "=" + x.Item2))}");
        }

        /// <summary>
        /// After the user filled out the Netatmo login form and was redirected to
        /// the configured callback, this method can be called to complete the login
        /// flow. If this method returns false, an error occured and the user could not
        /// be logged in. If true is returned, the resulting access and refresh tokens
        /// are stored in the connection provider and automatically used in all subsequent
        /// requests. Access tokens are also automatically renewed if expired.
        /// </summary>
        public async Task<bool> TryCompleteLogin(string state, string code)
        {
            try
            {
                await LoadInitialAccessToken(state, code);
                return _provider.AuthData.IsAccessTokenValid();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Calls the given endpoint at my-api.sonnen.de and ensures the request is authenticated.
        /// If no valid access token is present yet, this method will trigger a fresh sign-in or
        /// refresh the token with a refresh token if present and
        /// </summary>
        protected async Task<HttpResponseMessage> RequestNetatmoApi(Uri uri, IEnumerable<(string, string?)>? parameters = null)
        {
            await Authenticate();

            if (_authData.AccessToken is null)
                throw new IOException("Can not request data without access token.");

            var authparam = new[] { ("access_token", _authData.AccessToken!) };
            var postData = FormContentFromList(parameters == null ? authparam : authparam.Union(parameters.Where(p => p.Item2 != null).Select(p => (p.Item1, p.Item2!))));
            return await _client.PostAsync(uri, postData);
        }

        /// <summary>
        /// Calls the given endpoint at the netatmo api and ensures the request is authenticated
        /// and parses the result afterwards. If no valid access token is present yet, this method
        /// will trigger a fresh sign-in or refresh the token with a refresh token if present and
        /// append it to the request.
        /// </summary>
        protected async Task<TData> CallNetatmoApi<TWiremessage, TData>(Uri uri, IEnumerable<(string, string?)>? parameters = null)
            where TWiremessage : IWiremessage<TData> where TData : class
        {
            var response = await RequestNetatmoApi(uri, parameters);
            var responseStream = await response.Content.ReadAsStreamAsync();

            TWiremessage? result;
            try
            {
                result = await JsonSerializer.DeserializeAsync<TWiremessage>(responseStream, _jsonSerializerOptions);
            }
            catch (JsonException)
            {
                throw new IOException($"The API response could not be deserialized. HTTP status code: {(int)response.StatusCode}");
            }

            if (!response.IsSuccessStatusCode && result?.Error?.Code == 26)
                throw new IOException("API rate limit reached, see https://dev.netatmo.com/en-US/resources/technical/guides/ratelimits");

            if (!response.IsSuccessStatusCode)
                throw new IOException($"The API response was not succesful and returned with HTTP status code: {(int)response.StatusCode}, API error code: {result?.Error?.Code} and error message: '{result?.Error?.Message ?? string.Empty}'");

            if (result?.Body is null)
                throw new IOException($"The API response is missing a payload.");

            return result.Body;
        }

        private static FormUrlEncodedContent FormContentFromList(IEnumerable<(string, string)> values)
        {
            return new FormUrlEncodedContent(values.Select(x => new KeyValuePair<string?, string?>(x.Item1, x.Item2)));
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

                if (_authData.IsAccessTokenValid())
                    return;

                await RefreshAccessToken();

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

        private async Task LoadInitialAccessToken(string state, string code)
        {
            if (state != _state)
                throw new IOException("Unexpected error, state nonce was changed.");

            await LoadAndStoreAccessToken(new[]
            {
                ("grant_type", "authorization_code"),
                ("client_id", _provider.AppId),
                ("client_secret", _provider.AppSecret),
                ("code", code),
                ("redirect_uri", _provider.RedirectUri),
                ("scope", _provider.Scope)
            });
        }

        private async Task RefreshAccessToken()
        {
            if (_authData.RefreshToken is null)
                throw new IOException("Can not refresh access token without refresh token.");

            await LoadAndStoreAccessToken(new[]
            {
                ("grant_type", "refresh_token"),
                ("refresh_token", _authData.RefreshToken),
                ("client_id", _provider.AppId),
                ("client_secret", _provider.AppSecret),
            });
        }

        private async Task LoadAndStoreAccessToken(IEnumerable<(string, string)> postParameters)
        {
            var postData = FormContentFromList(postParameters);
            var responseMessage = await _client.PostAsync(new Uri(new Uri(_baseUri), "/oauth2/token"), postData);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            var responseData = await JsonSerializer.DeserializeAsync<AccessTokenResponse>(responseStream, _jsonSerializerOptions);

            await _authData.UpdateTokenAsync(responseData?.AccessToken, DateTime.UtcNow.AddSeconds(responseData?.ExpiresIn ?? 0), responseData?.RefreshToken);
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}