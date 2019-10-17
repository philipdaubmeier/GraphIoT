using System;
using System.Net.Http;

namespace PhilipDaubmeier.ViessmannClient.Model
{
    public class ViessmannConnectionProvider<T> : IViessmannConnectionProvider<T>, IDisposable
    {
        private bool skipDisposingClient = false;
        private bool skipDisposingAuthClient = false;
        private HttpClient client;
        private HttpClient authClient;

        /// <summary>
        /// See <see cref="IViessmannConnectionProvider{T}.AuthData"/>
        /// </summary>
        public IViessmannAuth AuthData { get; set; }

        /// <summary>
        /// See <see cref="IViessmannConnectionProvider{T}.Client"/>
        /// </summary>
        public HttpClient Client
        {
            get => client ?? (client = new HttpClient(CreateAuthHandler()));
            protected set
            {
                client = value;
                skipDisposingClient = true;
            }
        }

        /// <summary>
        /// See <see cref="IViessmannConnectionProvider{T}.AuthClient"/>
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
        /// For authentication on Viessmann Plattform we need a separate
        /// client that does not follow redirects. Use this handler for that
        /// purpose.
        /// </summary>
        public static HttpMessageHandler CreateAuthHandler() => new HttpClientHandler() { AllowAutoRedirect = false };

        public string VitotrolDeviceId { get; set; }
        public string VitotrolInstallationId { get; set; }

        public string PlattformInstallationId { get; set; }
        public string PlattformGatewayId { get; set; }
        public string PlattformApiClientId { get; set; }
        public string PlattformApiClientSecret { get; set; }

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