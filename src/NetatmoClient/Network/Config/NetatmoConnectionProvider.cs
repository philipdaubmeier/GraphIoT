using System;
using System.Net.Http;

namespace PhilipDaubmeier.NetatmoClient.Network
{
    public class NetatmoConnectionProvider : INetatmoConnectionProvider, IDisposable
    {
        private bool skipDisposingClient = false;
        private HttpClient? client = null;

        /// <summary>
        /// See <see cref="INetatmoConnectionProvider.AuthData"/>
        /// </summary>
        public INetatmoAuth AuthData { get; set; }

        /// <summary>
        /// See <see cref="INetatmoConnectionProvider.Client"/>
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

        public string AppId { get; set; } = string.Empty;

        public string AppSecret { get; set; } = string.Empty;

        public string Scope { get; set; } = string.Empty;

        public NetatmoConnectionProvider(INetatmoAuth authData)
            => AuthData = authData;

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