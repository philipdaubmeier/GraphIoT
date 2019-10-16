using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhilipDaubmeier.NetatmoClient.Model;
using PhilipDaubmeier.NetatmoClient.Model.Auth;
using PhilipDaubmeier.NetatmoClient.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoClient
{
    public abstract class NetatmoAuthBase : IDisposable
    {
        protected const string _baseUri = @"https://api.netatmo.net";

        private static readonly Semaphore _renewTokenSemaphore = new Semaphore(1, 1);

        private readonly INetatmoConnectionProvider _provider;
        private readonly INetatmoAuth _authData;

        private HttpMessageHandler _clientHandler;
        private HttpClient _client;

        private readonly JsonSerializer _jsonSerializer = new JsonSerializer()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public NetatmoAuthBase(INetatmoConnectionProvider connectionProvider)
        {
            _provider = connectionProvider;
            _authData = _provider?.AuthData;
            _clientHandler = _provider?.Handler ?? new HttpClientHandler();
            _client = new HttpClient(_clientHandler);
        }

        /// <summary>
        /// Calls the given endpoint at my-api.sonnen.de and ensures the request is authenticated.
        /// If no valid access token is present yet, this method will trigger a fresh sign-in or
        /// refresh the token with a refresh token if present and
        /// </summary>
        protected async Task<HttpResponseMessage> RequestNetatmoApi(Uri uri, IEnumerable<(string, string)> parameters = null)
        {
            await Authenticate();

            var authparam = new[] { ("access_token", _authData.AccessToken) };
            var postData = FormContentFromList(parameters == null ? authparam : authparam.Union(parameters.Where(p => p.Item2 != null)));
            return await _client.PostAsync(uri, postData);
        }

        /// <summary>
        /// Calls the given endpoint at the netatmo api and ensures the request is authenticated
        /// and parses the result afterwards. If no valid access token is present yet, this method
        /// will trigger a fresh sign-in or refresh the token with a refresh token if present and
        /// append it to the request.
        /// </summary>
        protected async Task<TData> CallNetatmoApi<TWiremessage, TData>(Uri uri, IEnumerable<(string, string)> parameters = null)
            where TWiremessage : IWiremessage<TData> where TData : class
        {
            var responseStream = await (await RequestNetatmoApi(uri, parameters)).Content.ReadAsStreamAsync();

            using var sr = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(sr);
            var result = _jsonSerializer.Deserialize<TWiremessage>(jsonTextReader)?.Body;
            if (result == null)
                throw new IOException("Could not deserialize response - most likely the rate limit was reached, see https://dev.netatmo.com/en-US/resources/technical/guides/ratelimits");

            return result;
        }

        private FormUrlEncodedContent FormContentFromList(IEnumerable<(string, string)> values)
        {
            return new FormUrlEncodedContent(values.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)));
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

                if (_authData.MustAuthenticate())
                    await GetInitialAccessToken();
                else if (_authData.MustRefreshToken())
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

        private async Task GetInitialAccessToken()
        {
            await LoadAndStoreAccessToken(new[]
            {
                ("grant_type", "password"),
                ("client_id", _provider.AppId),
                ("client_secret", _provider.AppSecret),
                ("username", _authData.Username),
                ("password", _authData.UserPassword),
                ("scope", _provider.Scope)
            });
        }

        private async Task RefreshAccessToken()
        {
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

            AccessTokenResponse responseData = null;
            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
                responseData = _jsonSerializer.Deserialize<AccessTokenResponse>(jsonTextReader);

            await _authData.UpdateTokenAsync(responseData.AccessToken, DateTime.UtcNow.AddSeconds(responseData.ExpiresIn), responseData.RefreshToken);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}