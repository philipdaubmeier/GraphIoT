using PhilipDaubmeier.DigitalstromClient.Network;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace PhilipDaubmeier.DigitalstromClient.Model
{
    public class DigitalstromConnectionProvider : IDigitalstromConnectionProvider
    {
        public UriPriorityList Uris { get; private set; }
        public IDigitalstromAuth AuthData { get; private set; }
        public X509Certificate2 ServerCertificate { get; private set; }
        public HttpMessageHandler Handler { get; private set; }

        public DigitalstromConnectionProvider(UriPriorityList uris, IDigitalstromAuth authData, X509Certificate2 cert = null, HttpMessageHandler handler = null)
        {
            Uris = uris;
            AuthData = authData;
            ServerCertificate = cert;
            Handler = handler;
        }
    }
}