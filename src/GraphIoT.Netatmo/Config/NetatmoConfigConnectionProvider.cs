using Microsoft.Extensions.Options;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Network;
using PhilipDaubmeier.TokenStore;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Config
{
    public class NetatmoConfigConnectionProvider : NetatmoConnectionProvider
    {
        public NetatmoConfigConnectionProvider(TokenStore<NetatmoWebClient> tokenStore, IOptions<NetatmoConfig> config, HttpClient client)
        {
            AppId = config.Value.AppId;
            AppSecret = config.Value.AppSecret;
            Scope = config.Value.Scope;

            AuthData = new NetatmoHostAuth(tokenStore, config.Value.Username, config.Value.Password);

            Client = client;
        }
    }
}