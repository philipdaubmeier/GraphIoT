using Microsoft.Extensions.Options;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Network;
using PhilipDaubmeier.TokenStore;
using System.Net.Http;

namespace PhilipDaubmeier.NetatmoHost.Config
{
    public class NetatmoConfigConnectionProvider : INetatmoConnectionProvider
    {
        public INetatmoAuth AuthData { get; private set; }
        public HttpMessageHandler Handler { get; private set; }

        public string AppId { get; private set; }
        public string AppSecret { get; private set; }
        public string Scope { get; private set; }

        public NetatmoConfigConnectionProvider(TokenStore<NetatmoWebClient> tokenStore, IOptions<NetatmoConfig> config)
        {
            AppId = config.Value.AppId;
            AppSecret = config.Value.AppSecret;
            Scope = config.Value.Scope;

            AuthData = new NetatmoHostAuth(tokenStore, config.Value.Username, config.Value.Password);
        }
    }
}