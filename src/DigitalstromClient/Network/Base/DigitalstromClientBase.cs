using Newtonsoft.Json;
using PhilipDaubmeier.DigitalstromClient.Model;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    public abstract class DigitalstromClientBase : IDisposable
    {
        private readonly JsonSerializer _jsonSerializer = new JsonSerializer();
        private readonly UriPriorityList _baseUris;
        private readonly HttpClient _client;
        private Task? _initializeTask;

        /// <summary>
        /// Provides fundamental functionality to request the Digitalstrom DSS REST webservice
        /// and parse the response JSON wireframe structure as well as selecting the first reachable
        /// uri from the UriPriorityList in the given connection provider.
        /// </summary>
        internal DigitalstromClientBase(IDigitalstromConnectionProvider connectionProvider)
        {
            _baseUris = connectionProvider.Uris.DeepClone();
            _client = connectionProvider.HttpClient;

            _initializeTask = Initialize();
        }

        /// <summary>
        /// Must be overwritten to execute the authentication process, e.g. fetch a token
        /// </summary>
        protected abstract Task Authenticate();

        /// <summary>
        /// Returns if the authentication has to be skipped, e.g. if a proxy already authenticates at the dss
        /// </summary>
        protected bool SkipAuthentication()
        {
            return _baseUris.CurrentHasAuthIncluded();
        }

        /// <summary>
        /// Tries to find the fist working baseUri in the list and if found, fetch a valid token
        /// </summary>
        private async Task Initialize()
        {
            _baseUris.First();
            while (!_baseUris.IsLast())
            {
                try
                {
                    var response = await _client.GetAsync(_baseUris.GetCurrent(), HttpCompletionOption.ResponseHeadersRead);
                    if (response.IsSuccessStatusCode)
                        return;
                }
                catch (Exception) { }

                _baseUris.MoveNext();
            }
        }

        protected async Task EnsureInitialized()
        {
            await GetBaseUri();
        }

        private async Task<Uri?> GetBaseUri()
        {
            if (_initializeTask == null)
                return _baseUris.GetCurrent();

            await _initializeTask;
            _initializeTask = null;

            return _baseUris.GetCurrent();
        }

        private async Task<Uri> BuildAbsoluteUri(Uri relativeUri)
        {
            var baseUri = await GetBaseUri();
            if (baseUri is null)
                throw new IOException("No base uri was given");
            else if (_baseUris.CurrentHasAuthIncluded())
                return new Uri($"{baseUri}{WebUtility.UrlEncode(relativeUri.ToString())}");
            else
                return new Uri(baseUri, relativeUri);
        }

        protected async Task<T> Load<T>(Uri uri, bool hasPayload = true) where T : class, IWiremessagePayload
        {
            return await Load<T>(uri, hasPayload);
        }

        private protected async Task<T?> Load<T>(UriQueryStringBuilder uri, bool hasPayload = true) where T : class, IWiremessagePayload
        {
            var responseData = await LoadWiremessage<T>(uri);

            if (responseData is null)
                throw new FormatException("No response data received");

            if (!responseData.Ok)
            {
                throw new IOException(string.Format("Received ok=false in DSS API! Message: \"{0}\"",
                    responseData.Message ?? "null"));
            }

            if (hasPayload && responseData.Result is null)
                throw new IOException("Received ok=true but no result object!");

            return responseData.Result;
        }

        private protected async Task<Wiremessage<T>?> LoadWiremessage<T>(UriQueryStringBuilder uri) where T : class, IWiremessagePayload
        {
            var requestUri = await BuildAbsoluteUri(uri);
            var responseMessage = await _client.GetAsync(requestUri);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            using var sr = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(sr);
            return _jsonSerializer.Deserialize<Wiremessage<T>>(jsonTextReader);
        }

        public void Dispose()
        {
            // explicit no-op. IDisposable is kept for compatibility reasons.
            // HttpClient must be disposed by IDigitalstromConnectionProvider.
        }
    }
}