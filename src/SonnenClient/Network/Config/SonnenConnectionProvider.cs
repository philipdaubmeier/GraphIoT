using System;
using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.SonnenClient.Network
{
    public class SonnenConnectionProvider : ISonnenConnectionProvider, IDisposable
    {
        private bool skipDisposingClient = false;
        private HttpClient? client;

        private static readonly CookieContainer cookieContainer = new CookieContainer();

        /// <summary>
        /// See <see cref="ISonnenConnectionProvider.AuthData"/>
        /// </summary>
        public ISonnenAuth AuthData { get; set; }

        /// <summary>
        /// See <see cref="ISonnenConnectionProvider.Client"/>
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
        /// For authentication on my.sonnen.de portal we need a handler that supports cookies.
        /// Always use this as inner handler if you create an own HttpClient to inject.
        /// </summary>
        public static HttpMessageHandler CreateHandler() => new HttpClientHandler()
        {
            UseCookies = true,
            CookieContainer = cookieContainer
        };

        public string ClientId { get; set; }

        public SonnenConnectionProvider(ISonnenAuth authData, string clientId)
            => (AuthData, ClientId) = (authData, clientId);

        #region IDisposable Support
        private bool disposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            if (!skipDisposingClient)
                client?.Dispose();

            disposed = true;
        }
        #endregion
    }
}