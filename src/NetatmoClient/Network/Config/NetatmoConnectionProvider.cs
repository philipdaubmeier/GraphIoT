using System;
using System.Net.Http;

namespace PhilipDaubmeier.NetatmoClient.Network
{
    public class NetatmoConnectionProvider : INetatmoConnectionProvider, IDisposable
    {
        private bool skipDisposingClient = false;
        private HttpClient client;

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

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string Scope { get; set; }

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