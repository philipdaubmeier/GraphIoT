using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace PhilipDaubmeier.DigitalstromClient
{
    public class DigitalstromConnectionProvider : IDigitalstromConnectionProvider
    {
        public UriPriorityList Uris { get; private set; }
        public IDigitalstromAuth AuthData { get; private set; }
        public X509Certificate2 ServerCertificate { get; private set; }
        public HttpMessageHandler Handler { get; private set; }

        public DigitalstromConnectionProvider(Uri uri, IDigitalstromAuth authData, X509Certificate2 cert = null, HttpMessageHandler handler = null)
            : this(new UriPriorityList(new List<Uri>() { uri }), authData, cert, handler)
        { }

        public DigitalstromConnectionProvider(UriPriorityList uris, IDigitalstromAuth authData, X509Certificate2 cert = null, HttpMessageHandler handler = null)
        {
            Uris = uris;
            AuthData = authData;
            ServerCertificate = cert;
            Handler = handler;
        }
    }
}