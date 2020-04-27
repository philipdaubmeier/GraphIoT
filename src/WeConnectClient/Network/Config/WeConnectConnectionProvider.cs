using System;
using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    public class WeConnectConnectionProvider : IWeConnectConnectionProvider, IDisposable
    {
        private bool skipDisposingClient = false;
        private bool skipDisposingAuthClient = false;
        private HttpClient? client = null;
        private HttpClient? authClient = null;

        public static readonly CookieContainer cookieContainer = new CookieContainer();

        /// <summary>
        /// See <see cref="IWeConnectConnectionProvider.AuthData"/>
        /// </summary>
        public IWeConnectAuth AuthData { get; set; }

        /// <summary>
        /// See <see cref="IWeConnectConnectionProvider.Client"/>
        /// </summary>
        public HttpClient Client
        {
            get => client ?? (client = new HttpClient(CreateHandler()));
            protected set
            {
                client = value;
                skipDisposingClient = true;
            }
        }

        /// <summary>
        /// See <see cref="IWeConnectConnectionProvider.AuthClient"/>
        /// </summary>
        public HttpClient AuthClient
        {
            get => authClient ?? (authClient = new HttpClient(CreateAuthHandler()));
            protected set
            {
                authClient = value;
                skipDisposingAuthClient = true;
            }
        }

        /// <summary>
        /// See <see cref="IWeConnectConnectionProvider.CookieContainer"/>
        /// </summary>
        public CookieContainer CookieContainer => cookieContainer;

        public static HttpClientHandler CreateHandler() => new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = cookieContainer
        };

        /// <summary>
        /// During authentication we need a separate client that does not
        /// follow redirects. Use this handler for that purpose.
        /// </summary>
        public static HttpClientHandler CreateAuthHandler() => new HttpClientHandler()
        {
            AllowAutoRedirect = false,
            UseCookies = true,
            CookieContainer = cookieContainer
        };

        public WeConnectConnectionProvider(IWeConnectAuth authData)
            => AuthData = authData;

        #region IDisposable Support
        private bool disposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            if (!skipDisposingClient)
                client?.Dispose();

            if (!skipDisposingAuthClient)
                authClient?.Dispose();

            disposed = true;
        }
        #endregion
    }
}