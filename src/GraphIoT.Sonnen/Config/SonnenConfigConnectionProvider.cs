using Microsoft.Extensions.Options;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenClient.Network;
using PhilipDaubmeier.TokenStore;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Config
{
    public class SonnenConfigConnectionProvider : ISonnenConnectionProvider
    {
        public ISonnenAuth AuthData { get; private set; }
        public HttpClient Client { get; private set; }

        public string ClientId { get; private set; }

        public SonnenConfigConnectionProvider(TokenStore<SonnenPortalClient> tokenStore, IOptions<SonnenConfig> config)
        {
            ClientId = config.Value.ClientId;

            AuthData = new SonnenHostAuth(tokenStore, config.Value.Username, config.Value.Password);
        }
    }
}