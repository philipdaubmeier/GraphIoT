using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace PhilipDaubmeier.DigitalstromClient
{
    public class DigitalstromConnectionProvider : IDigitalstromConnectionProvider, IDisposable
    {
        private static readonly Semaphore trustCertificateSemaphore = new Semaphore(1, 1);

        private readonly bool skipDisposingHttpClient = false;
        protected HttpClient httpClient { get; set; }
        protected HttpMessageHandler httpHandler { get; set; }

        /// <summary>
        /// See <see cref="IDigitalstromConnectionProvider.Uris"/>
        /// </summary>
        public UriPriorityList Uris { get; private set; }

        /// <summary>
        /// See <see cref="IDigitalstromConnectionProvider.AuthData"/>
        /// </summary>
        public IDigitalstromAuth AuthData { get; private set; }

        /// <summary>
        /// If this property is returning a valid X509Certificate, only responses from
        /// the server is accepted if it responds with exactly this server certificate
        /// (which may also be a self-signed one), i.e. a valid digitally signed cert
        /// with the same fingerprint, issuer and serial number. If it returns null,
        /// any valid cert is accepted but no self-signed ones of a Digitalstrom server.
        /// </summary>
        public X509Certificate2 ServerCertificate { get; set; }

        /// <summary>
        /// Gets or sets a callback method to validate the server certificate, which
        /// is called if no ServerCertificate is set.
        /// </summary>
        public Func<X509Certificate2, bool> ServerCertificateValidationCallback { get; private set; }

        /// <summary>
        /// A HttpMessageHandler to inject for the connection to the Digitalstrom server,
        /// either for mocking purposes for testing or also for setting a proxy server etc.
        /// If no one is set, a default handler will be created.
        /// </summary>
        public HttpMessageHandler Handler
        {
            get
            {
                if (httpHandler != null)
                    return httpHandler;

                return httpHandler = BuildHttpHandler();
            }
            set
            {
                if (httpHandler != null)
                    throw new InvalidOperationException("Handler is already set. Cannot be set twice.");

                httpHandler = value;
                httpHandler = BuildHttpHandler();
            }
        }

        /// <summary>
        /// See <see cref="IDigitalstromConnectionProvider.HttpClient"/>
        /// </summary>
        public HttpClient HttpClient
        {
            get
            {
                if (httpClient != null)
                    return httpClient;
                
                return httpClient = new HttpClient(Handler);
            }
            protected set
            {
                httpClient = value;
            }
        }
        
        public DigitalstromConnectionProvider(Uri uri, Func<IDigitalstromAuth> credentialCallback, Func<X509Certificate2, bool> certCallback, HttpMessageHandler handler = null)
            : this(new UriPriorityList(new List<Uri>() { uri }), credentialCallback, certCallback, handler)
        { }

        public DigitalstromConnectionProvider(Uri uri, IDigitalstromAuth authData, X509Certificate2 cert = null, HttpMessageHandler handler = null)
            : this(new UriPriorityList(new List<Uri>() { uri }), authData, cert, null, handler)
        { }

        public DigitalstromConnectionProvider(UriPriorityList uris, Func<IDigitalstromAuth> credentialCallback, Func<X509Certificate2, bool> certCallback, HttpMessageHandler handler = null)
            : this(uris, new EphemeralDigitalstromAuth(credentialCallback), null, certCallback, handler)
        { }

        public DigitalstromConnectionProvider(UriPriorityList uris, IDigitalstromAuth authData, X509Certificate2 cert = null, Func<X509Certificate2, bool> certCallback = null, HttpMessageHandler handler = null)
        {
            Uris = uris;
            AuthData = authData;
            ServerCertificate = cert;
            ServerCertificateValidationCallback = certCallback;

            if (handler != null)
                Handler = handler;
        }

        private HttpMessageHandler BuildHttpHandler()
        {
            var clientHandler = httpHandler ?? new HttpClientHandler();
            if (!(clientHandler is HttpClientHandler) ||
                (ServerCertificate is null && ServerCertificateValidationCallback is null))
                return clientHandler;

            (clientHandler as HttpClientHandler).ServerCertificateCustomValidationCallback = (request, cert, chain, sslPolicyErrors) =>
                sslPolicyErrors == SslPolicyErrors.None || ValidateCertificate(cert);
            return clientHandler;
        }

        private bool ValidateCertificate(X509Certificate2 cert)
        {
            if (!QueryCertificateTrusted(cert))
                return false;
            if (cert is null || ServerCertificate is null)
                return false;
            if (cert.Issuer != ServerCertificate.Issuer)
                return false;
            if (cert.GetSerialNumberString() != ServerCertificate.GetSerialNumberString())
                return false;
            if (cert.GetCertHashString() != ServerCertificate.GetCertHashString())
                return false;
            return true;
        }

        private bool QueryCertificateTrusted(X509Certificate2 cert)
        {
            if (!(ServerCertificate is null))
                return true;
            if (ServerCertificateValidationCallback is null)
                return true;

            try
            {
                trustCertificateSemaphore.WaitOne();
                if (!(ServerCertificate is null))
                    return true;

                if (!ServerCertificateValidationCallback(cert))
                    return false;

                ServerCertificate = cert;
                return true;
            }
            finally { trustCertificateSemaphore.Release(); }
        }

        #region IDisposable Support
        private bool disposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            if (!skipDisposingHttpClient)
                httpClient?.Dispose();

            disposed = true;
        }
        #endregion
    }
}