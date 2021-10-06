using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Model.Token;
using System;
using System.IO;
using System.Linq;
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

        private static readonly Semaphore _renewTokenSemaphore = new(1, 1);

        /// <summary>
        /// Connects to the Digitalstrom DSS REST webservice at the given uri with the given
        /// app and user credentials given via the IDigitalstromConnectionProvider object. If
        /// a valid application token is given in the auth data, it is used directly.
        /// This abstract base class handles the authentication flow including fetching and
        /// activating the application token, obtaining a session token as well as deserializing
        /// responses and unpacking the wiremessage.
        /// </summary>
        /// <param name="connectionProvider">All necessary connection infos like uris and
        /// authentication data needed to use for the webservice or to perform a new or
        /// renewed authentication</param>
        public DigitalstromAuthenticatingClientBase(IDigitalstromConnectionProvider connectionProvider)
            : base(connectionProvider)
        {
            _authData = connectionProvider.AuthData;
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
            var payload = await Load<T>(uri, true);

            if (payload is null)
                throw new IOException("Received ok=true but no result payload could be parsed!");

            return payload;
        }

        private protected async Task Load(UriQueryStringBuilder uri)
        {
            await Load<VoidPayload>(uri, false);
        }

        private protected new async Task<T?> Load<T>(UriQueryStringBuilder uri, bool hasPayload = true) where T : class, IWiremessagePayload
        {
            await Authenticate();

            if (!SkipAuthentication())
            {
                var sessionToken = _authData.SessionToken;
                if (sessionToken is null)
                    throw new ArgumentException("No session token present for connecting");

                uri = uri.AddQuery("token", sessionToken);
            }

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
            await EnsureInitialized();

            try
            {
                _renewTokenSemaphore.WaitOne();

                // needs no auth, url contains auth info already
                if (SkipAuthentication())
                    return;

                // we have a valid, not expired session token
                if (!_authData.MustFetchSessionToken())
                    return;

                // try fetching a session token or retry to get a fresh application token
                foreach (var _ in Enumerable.Range(0, 2))
                {
                    // fetch an application token first, if not present already
                    if (_authData.MustFetchApplicationToken())
                    {
                        await FetchApplicationToken();
                        await ActivateApplicationToken(await LoginCredentials());
                    }

                    // try to refresh the session token if the application token is activated
                    if (await RefreshSessionToken() && !_authData.MustFetchSessionToken())
                        return;
                }

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

            var appToken = responseData?.Result?.ApplicationToken;
            if (appToken is null)
                throw new IOException("Could not get an application token");
            
            await _authData.UpdateTokenAsync(null, DateTime.MinValue, appToken);
        }

        private async Task<string> LoginCredentials()
        {
            Uri uri = new Uri("/json/system/login", UriKind.Relative)
                .AddQuery("user", _authData.Username)
                .AddQuery("password", _authData.UserPassword);

            var responseData = await LoadWiremessage<SessionTokenResponse>(uri);

            if (responseData is null)
                throw new FormatException("No response data received");
            if (responseData.Ok && responseData.Result?.Token != null)
                return responseData.Result.Token;
            throw new IOException("Could not log in");
        }

        private async Task ActivateApplicationToken(string loginSessionToken)
        {
            if (_authData.MustFetchApplicationToken() || string.IsNullOrEmpty(loginSessionToken))
                throw new ArgumentException("Application token and temporary session token must be present before getting a new session token");

            var appToken = _authData.ApplicationToken;
            if (appToken is null)
                throw new ArgumentException("Application token could not be loaded for activating it");

            Uri uri = new Uri("/json/system/enableToken", UriKind.Relative)
                .AddQuery("applicationToken", appToken)
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

            var appToken = _authData.ApplicationToken;
            if (appToken is null)
                throw new ArgumentException("Application token could not be loaded for refreshing session token");

            Uri uri = new Uri("/json/system/loginApplication", UriKind.Relative)
                .AddQuery("loginToken", appToken);

            var responseData = await LoadWiremessage<SessionTokenResponse>(uri);

            if (responseData == null)
                throw new FormatException("No response data received");

            if (responseData.Ok && responseData.Result != null && !string.IsNullOrEmpty(responseData.Result.Token))
            {
                await _authData.UpdateTokenAsync(responseData.Result.Token, DateTime.UtcNow.AddSeconds(60), appToken);
                return true;
            }

            // Application token was invalidated, trigger complete fresh login
            if (responseData.Message != null && responseData.Message.Equals("Application authentication failed", StringComparison.OrdinalIgnoreCase))
            {
                await _authData.UpdateTokenAsync(string.Empty, DateTime.MinValue, string.Empty);
                return false;
            }

            // In this case, a retry does not seem to be a solution
            throw new IOException("Could not get session token");
        }
    }
}