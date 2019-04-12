using DigitalstromClient.Model;
using DigitalstromClient.Model.Auth;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DigitalstromClient.Network
{
    public abstract class AbstractDigitalstromClient
    {
        protected UriPriorityList _baseUris;
        protected bool isLogging = false;
        protected string log;
        private X509Certificate2 _dssCert;
        private IDigitalstromAuth _authData;
        private HttpMessageHandler _clientHandler;
        private HttpClient _client;
        private Task _initializeTask;

        /// <summary>
        /// Connects to the Digitalstrom DSS REST webservice at the given uri with the given
        /// app and user credentials given via the authentication data object. If
        /// a valid application token is given in the auth data, it is used directly.
        /// This abstract base class handles establishing the TLS connection with accepting
        /// the self signed DSS certificate as was as the authentication flow including
        /// fetching and activating the application token and obtaining a session token.
        /// </summary>
        /// <param name="connectionProvider">All necessary connection infos like uris and
        /// authentication data needed to use for the webservice or to perform a new or
        /// renewed authentication</param>
        public AbstractDigitalstromClient(IDigitalstromConnectionProvider connectionProvider)
        {
            _baseUris = connectionProvider.Uris.DeepClone();
            _authData = connectionProvider.AuthData;
            _dssCert = connectionProvider.ServerCertificate;
            
            _clientHandler = connectionProvider.Handler ?? new HttpClientHandler();
            if (_clientHandler is HttpClientHandler && _dssCert != null)
            {
                (_clientHandler as HttpClientHandler).ServerCertificateCustomValidationCallback = (request, cert, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                        return true; // certificate is valid anyways
                    if (cert == null)
                        return false;
                    if (cert.Issuer != _dssCert.Issuer)
                        return false;
                    if (cert.GetSerialNumberString() != _dssCert.GetSerialNumberString())
                        return false;
                    if (cert.GetCertHashString() != _dssCert.GetCertHashString())
                        return false;
                    return true;
                };
            }

            _client = new HttpClient(_clientHandler);

            _initializeTask = Initialize();
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

        protected async Task<Uri> BuildAbsoluteUri(Uri relativeUri)
        {
            var baseUri = await GetBaseUri();
            if (_baseUris.CurrentHasAuthIncluded())
                return new Uri($"{baseUri}{WebUtility.UrlEncode(relativeUri.ToString())}");
            else
                return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// Loads the given generic type from the given uri.
        /// </summary>
        /// <typeparam name="T">
        /// Generic type to deserialize the response into - 
        /// has to be derived from IWiremessagePayload
        /// </typeparam>
        /// <param name="uri">Uri of the API interface to call</param>
        /// <returns>The deserialized response object</returns>
        protected async Task<T> Load<T>(UriQueryStringBuilder uri) where T : IWiremessagePayload<T>
        {
            return await Load<T>(uri, true);
        }

        private class VoidPayload : IWiremessagePayload<VoidPayload> { }

        /// <summary>
        /// Calls the given API interface, which does not return a response payload.
        /// </summary>
        /// <param name="uri">Uri of the API interface to call</param>
        protected async Task Load(UriQueryStringBuilder uri)
        {
            await Load<VoidPayload>(uri, false);
        }

        /// <summary>
        /// Ensures that after the completion of this task, a valid non-expired session token is
        /// present in the authentication data object, or throws an exception if unsuccessful.
        /// </summary>
        protected async Task Authenticate()
        {
            // needs no auth, url contains auth info already
            if (_baseUris.CurrentHasAuthIncluded())
                return;

            // we have a valid, not expired session token
            if (!_authData.MustFetchSessionToken())
                return;

            // fetch an application token first, if not present already
            if (_authData.MustFetchApplicationToken())
            {
                await FetchApplicationToken();
                await ActivateApplicationToken(await LoginCredentials());
            }

            // try to refresh the session token if the application token is activated
            if (await RefreshSessionToken() && !_authData.MustFetchSessionToken())
                return;

            throw new IOException("Could not authenticate");
        }

        private async Task FetchApplicationToken()
        {
            Uri uri = new Uri(_baseUris.GetCurrent(), "/json/system/requestApplicationToken")
                .AddQuery("applicationName", _authData.AppId);

            var responseData = await LoadWiremessage<ApplicationTokenResponse>(uri);

            var appToken = responseData == null || responseData.result == null ? null : responseData.result.applicationToken;
            await _authData.UpdateTokenAsync(null, DateTime.MinValue, appToken);

            if (_authData.ApplicationToken == null)
                throw new IOException("Could not get an application token");
        }

        private async Task<string> LoginCredentials()
        {
            Uri uri = new Uri(_baseUris.GetCurrent(), "/json/system/login")
                .AddQuery("user", _authData.Username)
                .AddQuery("password", _authData.UserPassword);

            var responseData = await LoadWiremessage<SessionTokenResponse>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");
            if (responseData.ok && responseData.result != null)
                return responseData.result.token;
            throw new IOException("Could not log in");
        }

        private async Task ActivateApplicationToken(string loginSessionToken)
        {
            if (_authData.MustFetchApplicationToken() || string.IsNullOrEmpty(loginSessionToken))
                throw new ArgumentException("Application token and temporary session token must be present before getting a new session token");

            Uri uri = new Uri(_baseUris.GetCurrent(), "/json/system/enableToken")
                .AddQuery("applicationToken", _authData.ApplicationToken)
                .AddQuery("token", loginSessionToken);

            var responseData = await LoadWiremessage<LoginResponse>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");
            if (!responseData.ok)
                throw new IOException(string.Format("Could not activate application token. Message: {0}", responseData.message));
        }

        private async Task<bool> RefreshSessionToken()
        {
            if (_authData.MustFetchApplicationToken())
                throw new ArgumentException("Application token must be present before getting a session token");

            Uri uri = new Uri(_baseUris.GetCurrent(), "/json/system/loginApplication")
                .AddQuery("loginToken", _authData.ApplicationToken);

            var responseData = await LoadWiremessage<SessionTokenResponse>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");

            if (responseData.ok && responseData.result != null && !string.IsNullOrEmpty(responseData.result.token))
            {
                await _authData.UpdateTokenAsync(responseData.result.token, DateTime.UtcNow.AddSeconds(60), _authData.ApplicationToken);
                return true;
            }

            // Must login first, try login of the application token
            if (responseData.message != null && responseData.message.Equals("Application-Authentication failed", StringComparison.OrdinalIgnoreCase))
                return false;

            // In this case, a retry does not seem to be a solution
            throw new IOException("Could not get session token");
        }

        private async Task<IWiremessagePayload<T>.Wiremessage> LoadWiremessage<T>(Uri uri) where T : IWiremessagePayload<T>
        {
            if (isLogging)
                log += (await (await _client.GetAsync(uri)).Content.ReadAsStringAsync()) + "\n";

            var responseMessage = await _client.GetAsync(uri);
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            
            using (var sr = new StreamReader(responseStream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<IWiremessagePayload<T>.Wiremessage>(jsonTextReader);
            }
        }

        private async Task<T> Load<T>(UriQueryStringBuilder uri, bool hasPayload) where T : IWiremessagePayload<T>
        {
            await Authenticate();

            if (!_baseUris.CurrentHasAuthIncluded())
                uri = uri.AddQuery("token", _authData.SessionToken);

            var requestUri = await BuildAbsoluteUri(uri);
            IWiremessagePayload<T>.Wiremessage responseData = await LoadWiremessage<T>(requestUri);

            if (responseData == null)
                throw new FormatException("No response data received");

            if (!responseData.ok)
            {
                throw new IOException(string.Format("Received ok=false in DSS API! Message: \"{0}\"",
                    responseData.message == null ? "null" : responseData.message));
            }

            if (hasPayload && responseData.result == null)
                throw new IOException("Received ok=true but no result object!");

            // only touch token if successful
            if (!_baseUris.CurrentHasAuthIncluded())
                await _authData.TouchSessionTokenAsync();

            return responseData.result;
        }
    }
}