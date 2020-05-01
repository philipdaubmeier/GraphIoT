using Microsoft.Extensions.Options;
using PhilipDaubmeier.WeConnectClient;
using PhilipDaubmeier.WeConnectClient.Network;
using PhilipDaubmeier.TokenStore;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.WeConnect.Config
{
    public class WeConnectHttpClient
    {
        public WeConnectHttpClient(HttpClient client) => Client = client;
        public HttpClient Client { get; private set; }
    }

    public class WeConnectAuthHttpClient
    {
        public WeConnectAuthHttpClient(HttpClient authClient) => AuthClient = authClient;
        public HttpClient AuthClient { get; private set; }
    }

    public class WeConnectConfigConnectionProvider : WeConnectConnectionProvider
    {
        public WeConnectConfigConnectionProvider(TokenStore<WeConnectPortalClient> tokenStore, IOptions<WeConnectConfig> config, WeConnectHttpClient client, WeConnectAuthHttpClient authClient)
            : base(new WeConnectHostAuth(tokenStore, config.Value.Username, config.Value.Password))
            => (Client, AuthClient) = (client.Client, authClient.AuthClient);
    }
}