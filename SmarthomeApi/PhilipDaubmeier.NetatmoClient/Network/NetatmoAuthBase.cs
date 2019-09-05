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
    public abstract class NetatmoAuthBase
    {
        protected const string _baseUri = @"https://api.netatmo.net";

        private static readonly Semaphore _renewTokenSemaphore = new Semaphore(1, 1);

        private readonly INetatmoConnectionProvider _provider;
        private readonly INetatmoAuth _authData;

        private static HttpMessageHandler _clientHandler;
        private static HttpClient _client;

        private static readonly IContractResolver _jsonResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer() { ContractResolver = _jsonResolver };
        
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
        protected async Task<HttpResponseMessage> RequestNetatmoApi(Uri uri, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            await Authenticate();

            var authparam = new[]
            {
                new KeyValuePair<string, string>("access_token", _authData.AccessToken)
            };
            var postData = new FormUrlEncodedContent(parameters == null ? authparam : authparam.Union(parameters.Where(p => p.Value != null)));
            return await _client.PostAsync(uri, postData);
        }

        /// <summary>
        /// Calls the given endpoint at the netatmo api and ensures the request is authenticated
        /// and parses the result afterwards. If no valid access token is present yet, this method
        /// will trigger a fresh sign-in or refresh the token with a refresh token if present and
        /// append it to the request.
        /// </summary>
        protected async Task<TData> CallNetatmoApi<TWiremessage, TData>(Uri uri, IEnumerable<KeyValuePair<string, string>> parameters = null)
            where TWiremessage : IWiremessage<TData> where TData : class
        {
            var responseStream = await (await RequestNetatmoApi(uri, parameters)).Content.ReadAsStreamAsync();

            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
                return _jsonSerializer.Deserialize<TWiremessage>(jsonTextReader).Body;
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
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", _provider.AppId),
                new KeyValuePair<string, string>("client_secret", _provider.AppSecret),
                new KeyValuePair<string, string>("username", _authData.Username),
                new KeyValuePair<string, string>("password", _authData.UserPassword),
                new KeyValuePair<string, string>("scope", _provider.Scope)
            });
        }

        private async Task RefreshAccessToken()
        {
            await LoadAndStoreAccessToken(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", _authData.RefreshToken),
                new KeyValuePair<string, string>("client_id", _provider.AppId),
                new KeyValuePair<string, string>("client_secret", _provider.AppSecret),
            });
        }

        private async Task LoadAndStoreAccessToken(IEnumerable<KeyValuePair<string, string>> postParameters)
        {
            var postData = new FormUrlEncodedContent(postParameters);
            var responseMessage = await _client.PostAsync(new Uri(new Uri(_baseUri), "/oauth2/token"), postData);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            AccessTokenResponse responseData = null;
            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
                responseData = _jsonSerializer.Deserialize<AccessTokenResponse>(jsonTextReader);

            await _authData.UpdateTokenAsync(responseData.AccessToken, DateTime.UtcNow.AddSeconds(responseData.ExpiresIn), responseData.RefreshToken);
        }
    }
}