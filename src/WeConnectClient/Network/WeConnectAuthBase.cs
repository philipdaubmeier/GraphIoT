using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    public abstract class WeConnectAuthBase
    {
        protected readonly IWeConnectConnectionProvider<WeConnectPortalClient> _connectionProvider;

        private readonly HttpClient _client;
        private readonly HttpClient _authClient;

        private const string _authUri = "https://identity.vwgroup.io";

        private static readonly Semaphore _renewTokenSemaphore = new Semaphore(1, 1);

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public WeConnectAuthBase(IWeConnectConnectionProvider<WeConnectPortalClient> connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _client = connectionProvider.Client;
            _authClient = connectionProvider.AuthClient;
        }

        /// <summary>
        /// Calls the given endpoint at the WeConnect Portal api and ensures the request is authenticated.
        /// </summary>
        protected async Task<HttpResponseMessage> RequestApi(Uri uri)
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
        /// Ensures that after the completion of this task, a valid non-expired access token is
        /// present in the authentication data object, or throws an exception if unsuccessful.
        /// </summary>
        protected async Task Authenticate()
        {
            try
            {
                _renewTokenSemaphore.WaitOne();


                throw new IOException("Could not authenticate");
            }
            finally
            {
                _renewTokenSemaphore.Release();
            }
        }

    }
}