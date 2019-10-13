using Microsoft.Extensions.Options;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Network;
using PhilipDaubmeier.TokenStore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Config
{
    public class DigitalstromConfigConnectionProvider : DigitalstromConnectionProvider
    {
        public DigitalstromConfigConnectionProvider(TokenStore<PersistingDigitalstromAuth> tokenStore, IOptions<DigitalstromConfig> config)
            : base(UrisFromConfig(config), AuthFromConfig(tokenStore, config), CertFromConfig(config))
        {
            if (!string.IsNullOrWhiteSpace(config.Value.Proxy) && int.TryParse(config.Value.ProxyPort, out int port))
            {
                Handler = new HttpClientHandler()
                {
                    UseProxy = true,
                    Proxy = new WebProxy(config.Value.Proxy, port)
                };
            }
        }

        private static IDigitalstromAuth AuthFromConfig(TokenStore<PersistingDigitalstromAuth> tokenStore, IOptions<DigitalstromConfig> config)
        {
            return new PersistingDigitalstromAuth(tokenStore, config.Value.TokenAppId, config.Value.DssUsername, config.Value.DssPassword);
        }

        private static X509Certificate2 CertFromConfig(IOptions<DigitalstromConfig> config)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(config.Value.DssCertificate))
                    return null;

                return new X509Certificate2(Convert.FromBase64String(config.Value.DssCertificate));
            }
            catch
            {
                return null;
            }
        }

        private static UriPriorityList UrisFromConfig(IOptions<DigitalstromConfig> config)
        {
            if (config.Value.UseCloudredir)
                return new UriPriorityList(new List<Uri>() { config.Value.UriCloudredir }, new List<bool>() { true });
            else if (config.Value.UriLocal != null)
                return new UriPriorityList(new List<Uri>() { config.Value.UriLocal, config.Value.UriDsNet });
            else
                return new UriPriorityList(new List<Uri>() { config.Value.UriDsNet });
        }
    }
}