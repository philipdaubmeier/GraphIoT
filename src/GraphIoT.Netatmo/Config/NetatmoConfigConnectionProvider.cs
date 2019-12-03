using Microsoft.Extensions.Options;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Network;
using PhilipDaubmeier.TokenStore;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Config
{
    public class NetatmoHttpClient
    {
        public NetatmoHttpClient(HttpClient client) => Client = client;
        public HttpClient Client { get; private set; }
    }

    public class NetatmoConfigConnectionProvider : NetatmoConnectionProvider
    {
        public NetatmoConfigConnectionProvider(TokenStore<NetatmoWebClient> tokenStore, IOptions<NetatmoConfig> config, NetatmoHttpClient client)
            : base(new NetatmoHostAuth(tokenStore, config.Value.Username, config.Value.Password))
            => (AppId, AppSecret, Scope, Client) = (config.Value.AppId, config.Value.AppSecret, config.Value.Scope, client.Client);
    }
}