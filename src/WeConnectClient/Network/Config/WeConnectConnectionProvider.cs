using System;
using System.Net.Http;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    public class WeConnectConnectionProvider<T> : IWeConnectConnectionProvider<T>, IDisposable
    {
        private bool skipDisposingClient = false;
        private bool skipDisposingAuthClient = false;
        private HttpClient? client = null;
        private HttpClient? authClient = null;

        /// <summary>
        /// See <see cref="IWeConnectConnectionProvider{T}.AuthData"/>
        /// </summary>
        public IWeConnectAuth AuthData { get; set; }

        /// <summary>
        /// See <see cref="IWeConnectConnectionProvider{T}.Client"/>
        /// </summary>
        public HttpClient Client
        {
            get => client ?? (client = new HttpClient());
            protected set
            {
                client = value;
                skipDisposingClient = true;
            }
        }

        /// <summary>
        /// See <see cref="IWeConnectConnectionProvider{T}.AuthClient"/>
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
        /// During authentication we need a separate client that does not
        /// follow redirects. Use this handler for that purpose.
        /// </summary>
        public static HttpClientHandler CreateAuthHandler() => new HttpClientHandler() { AllowAutoRedirect = false };

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