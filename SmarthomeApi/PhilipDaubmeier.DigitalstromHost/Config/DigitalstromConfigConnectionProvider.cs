using Microsoft.Extensions.Options;
using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Network;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.TokenStore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace PhilipDaubmeier.DigitalstromHost.Config
{
    public class DigitalstromConfigConnectionProvider : IDigitalstromConnectionProvider
    {
        public UriPriorityList Uris { get; private set; }
        public IDigitalstromAuth AuthData { get; private set; }
        public X509Certificate2 ServerCertificate { get; private set; }
        public HttpMessageHandler Handler { get; private set; }

        public DigitalstromConfigConnectionProvider(TokenStore<PersistingDigitalstromAuth> tokenStore, IDigitalstromDbContext db, IOptions<DigitalstromConfig> config)
        {
            if (!string.IsNullOrWhiteSpace(config.Value.Proxy) && int.TryParse(config.Value.ProxyPort, out int port))
            {
                Handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = new WebProxy(config.Value.Proxy, port)
                };
            }

            if (!string.IsNullOrWhiteSpace(config.Value.DssCertificate))
                ServerCertificate = new X509Certificate2(Convert.FromBase64String(config.Value.DssCertificate));

            if (config.Value.UseCloudredir)
                Uris = new UriPriorityList(new List<Uri>() { config.Value.UriCloudredir }, new List<bool>() { true });
            else if (config.Value.UriLocal != null)
                Uris = new UriPriorityList(new List<Uri>() { config.Value.UriLocal, config.Value.UriDsNet });
            else
                Uris = new UriPriorityList(new List<Uri>() { config.Value.UriDsNet });

            AuthData = new PersistingDigitalstromAuth(tokenStore, config.Value.TokenAppId, config.Value.DssUsername, config.Value.DssPassword);
        }
    }
}