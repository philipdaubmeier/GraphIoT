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
        private readonly UriPriorityList _baseUris;
        private readonly HttpClient _client;
        private Task _initializeTask;

        /// <summary>
        /// Provides fundamental functionality to request the Digitalstrom DSS REST webservice
        /// and parse the response JSON wireframe structure as well as selecting the first reachable
        /// uri from the given UriPriorityList.
        /// </summary>
        internal DigitalstromClientBase(UriPriorityList baseUris, HttpMessageHandler clientHandler = null)
        {
            _baseUris = baseUris.DeepClone();
            _client = new HttpClient(clientHandler ?? new HttpClientHandler());

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
                    {
                        await Authenticate();
                        return;
                    }
                }
                catch (Exception) { }

                _baseUris.MoveNext();
            }
        }

        private async Task<Uri> GetBaseUri()
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
            if (_baseUris.CurrentHasAuthIncluded())
                return new Uri($"{baseUri}{WebUtility.UrlEncode(relativeUri.ToString())}");
            else
                return new Uri(baseUri, relativeUri);
        }

        protected async Task<T> Load<T>(UriQueryStringBuilder uri, bool hasPayload = true) where T : IWiremessagePayload<T>
        {
            IWiremessagePayload<T>.Wiremessage responseData = await LoadWiremessage<T>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");

            if (!responseData.Ok)
            {
                throw new IOException(string.Format("Received ok=false in DSS API! Message: \"{0}\"",
                    responseData.Message ?? "null"));
            }

            if (hasPayload && responseData.Result == null)
                throw new IOException("Received ok=true but no result object!");

            return responseData.Result;
        }

        protected async Task<IWiremessagePayload<T>.Wiremessage> LoadWiremessage<T>(UriQueryStringBuilder uri) where T : IWiremessagePayload<T>
        {
            var responseMessage = await _client.GetAsync(await BuildAbsoluteUri(uri));
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();

            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<IWiremessagePayload<T>.Wiremessage>(jsonTextReader);
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}