using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Model.Token;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    public abstract class DigitalstromAuthenticatingClientBase : DigitalstromClientBase
    {
        /// <summary>
        /// Needed internally for wiremessages without return value.
        /// </summary>
        private class VoidPayload : IWiremessagePayload { }

        private readonly IDigitalstromAuth _authData;

        private static readonly Semaphore _trustCertificateSemaphore = new Semaphore(1, 1);
        private static readonly Semaphore _renewTokenSemaphore = new Semaphore(1, 1);

        /// <summary>
        /// Connects to the Digitalstrom DSS REST webservice at the given uri with the given
        /// app and user credentials given via the IDigitalstromConnectionProvider object. If
        /// a valid application token is given in the auth data, it is used directly.
        /// This abstract base class handles establishing the TLS connection with accepting
        /// the self signed DSS certificate, the authentication flow including fetching and
        /// activating the application token, obtaining a session token as well as deserializing
        /// responses and unpacking the wiremessage.
        /// </summary>
        /// <param name="connectionProvider">All necessary connection infos like uris and
        /// authentication data needed to use for the webservice or to perform a new or
        /// renewed authentication</param>
        public DigitalstromAuthenticatingClientBase(IDigitalstromConnectionProvider connectionProvider)
            : base(connectionProvider.Uris, BuildHttpHandler(connectionProvider))
        {
            _authData = connectionProvider.AuthData;
        }

        private static HttpMessageHandler BuildHttpHandler(IDigitalstromConnectionProvider connectionProvider)
        {
            var clientHandler = connectionProvider.Handler ?? new HttpClientHandler();
            if (!(clientHandler is HttpClientHandler) || 
                (connectionProvider.ServerCertificate is null && connectionProvider.ServerCertificateValidationCallback is null))
                return clientHandler;

            (clientHandler as HttpClientHandler).ServerCertificateCustomValidationCallback = (request, cert, chain, sslPolicyErrors) =>
                sslPolicyErrors == SslPolicyErrors.None || ValidateCertificate(connectionProvider, cert);
            return clientHandler;
        }

        private static bool ValidateCertificate(IDigitalstromConnectionProvider connectionProvider, X509Certificate2 cert)
        {
            if (!QueryCertificateTrusted(connectionProvider, cert))
                return false;
            if (cert is null || connectionProvider.ServerCertificate is null)
                return false;
            if (cert.Issuer != connectionProvider.ServerCertificate.Issuer)
                return false;
            if (cert.GetSerialNumberString() != connectionProvider.ServerCertificate.GetSerialNumberString())
                return false;
            if (cert.GetCertHashString() != connectionProvider.ServerCertificate.GetCertHashString())
                return false;
            return true;
        }

        private static bool QueryCertificateTrusted(IDigitalstromConnectionProvider connectionProvider, X509Certificate2 cert)
        {
            if (!(connectionProvider.ServerCertificate is null))
                return true;
            if (connectionProvider.ServerCertificateValidationCallback is null)
                return true;

            try
            {
                _trustCertificateSemaphore.WaitOne();
                if (!(connectionProvider.ServerCertificate is null))
                    return true;
                
                if (!connectionProvider.ServerCertificateValidationCallback(cert))
                    return false;

                connectionProvider.ServerCertificate = cert;
                return true;
            }
            finally { _trustCertificateSemaphore.Release(); }
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
        protected async Task<T> Load<T>(Uri uri) where T : class, IWiremessagePayload
        {
            return await Load<T>(new UriQueryStringBuilder(uri));
        }

        /// <summary>
        /// Calls the given API interface, which does not return a response payload.
        /// </summary>
        /// <param name="uri">Uri of the API interface to call</param>
        protected async Task Load(Uri uri)
        {
            await Load(new UriQueryStringBuilder(uri));
        }

        private protected async Task<T> Load<T>(UriQueryStringBuilder uri) where T : class, IWiremessagePayload
        {
            return await Load<T>(uri, true);
        }

        private protected async Task Load(UriQueryStringBuilder uri)
        {
            await Load<VoidPayload>(uri, false);
        }

        private protected new async Task<T> Load<T>(UriQueryStringBuilder uri, bool hasPayload = true) where T : class, IWiremessagePayload
        {
            await Authenticate();

            if (!SkipAuthentication())
                uri = uri.AddQuery("token", _authData.SessionToken);

            var result = await base.Load<T>(uri, hasPayload);

            // only touch token if successful
            if (!SkipAuthentication())
                await _authData.TouchSessionTokenAsync();

            return result;
        }

        /// <summary>
        /// Ensures that after the completion of this task, a valid non-expired session token is
        /// present in the authentication data object, or throws an exception if unsuccessful.
        /// </summary>
        protected override async Task Authenticate()
        {
            try
            {
                _renewTokenSemaphore.WaitOne();

                // needs no auth, url contains auth info already
                if (SkipAuthentication())
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
            finally
            {
                _renewTokenSemaphore.Release();
            }
        }

        private async Task FetchApplicationToken()
        {
            Uri uri = new Uri("/json/system/requestApplicationToken", UriKind.Relative)
                .AddQuery("applicationName", _authData.AppId);

            var responseData = await LoadWiremessage<ApplicationTokenResponse>(uri);

            var appToken = responseData == null || responseData.Result == null ? null : responseData.Result.ApplicationToken;
            await _authData.UpdateTokenAsync(null, DateTime.MinValue, appToken);

            if (_authData.ApplicationToken == null)
                throw new IOException("Could not get an application token");
        }

        private async Task<string> LoginCredentials()
        {
            Uri uri = new Uri("/json/system/login", UriKind.Relative)
                .AddQuery("user", _authData.Username)
                .AddQuery("password", _authData.UserPassword);

            var responseData = await LoadWiremessage<SessionTokenResponse>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");
            if (responseData.Ok && responseData.Result != null)
                return responseData.Result.Token;
            throw new IOException("Could not log in");
        }

        private async Task ActivateApplicationToken(string loginSessionToken)
        {
            if (_authData.MustFetchApplicationToken() || string.IsNullOrEmpty(loginSessionToken))
                throw new ArgumentException("Application token and temporary session token must be present before getting a new session token");

            Uri uri = new Uri("/json/system/enableToken", UriKind.Relative)
                .AddQuery("applicationToken", _authData.ApplicationToken)
                .AddQuery("token", loginSessionToken);

            var responseData = await LoadWiremessage<LoginResponse>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");
            if (!responseData.Ok)
                throw new IOException(string.Format("Could not activate application token. Message: {0}", responseData.Message));
        }

        private async Task<bool> RefreshSessionToken()
        {
            if (_authData.MustFetchApplicationToken())
                throw new ArgumentException("Application token must be present before getting a session token");

            Uri uri = new Uri("/json/system/loginApplication", UriKind.Relative)
                .AddQuery("loginToken", _authData.ApplicationToken);

            var responseData = await LoadWiremessage<SessionTokenResponse>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");

            if (responseData.Ok && responseData.Result != null && !string.IsNullOrEmpty(responseData.Result.Token))
            {
                await _authData.UpdateTokenAsync(responseData.Result.Token, DateTime.UtcNow.AddSeconds(60), _authData.ApplicationToken);
                return true;
            }

            // Must login first, try login of the application token
            if (responseData.Message != null && responseData.Message.Equals("Application-Authentication failed", StringComparison.OrdinalIgnoreCase))
                return false;

            // In this case, a retry does not seem to be a solution
            throw new IOException("Could not get session token");
        }
    }
}