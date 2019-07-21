using Microsoft.Extensions.Options;
using PhilipDaubmeier.SmarthomeApi.Model.Config;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenClient.Model;
using PhilipDaubmeier.TokenStore;
using System.Net.Http;

namespace PhilipDaubmeier.SmarthomeApi.Model
{
    public class SonnenConfigConnectionProvider : ISonnenConnectionProvider
    {
        public ISonnenAuth AuthData { get; private set; }
        public HttpMessageHandler Handler { get; private set; }
        
        public string ClientId { get; private set; }

        public SonnenConfigConnectionProvider(TokenStore<SonnenPortalClient> tokenStore, IOptions<SonnenConfig> config)
        {
            ClientId = config.Value.ClientId;
            
            AuthData = new SonnenHostAuth(tokenStore, config.Value.Username, config.Value.Password);
        }
    }
}