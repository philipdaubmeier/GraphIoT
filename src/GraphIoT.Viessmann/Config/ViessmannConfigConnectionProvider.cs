using Microsoft.Extensions.Options;
using PhilipDaubmeier.TokenStore;
using PhilipDaubmeier.ViessmannClient.Model;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannHttpClient<T>
    {
        public ViessmannHttpClient(HttpClient client) => Client = client;
        public HttpClient Client { get; private set; }
    }

    public class ViessmannAuthHttpClient
    {
        public ViessmannAuthHttpClient(HttpClient authClient) => AuthClient = authClient;
        public HttpClient AuthClient { get; private set; }
    }

    public class ViessmannConfigConnectionProvider<T> : ViessmannConnectionProvider<T>
    {
        public ViessmannConfigConnectionProvider(TokenStore<T> tokenStore, IOptions<ViessmannConfig> config, ViessmannHttpClient<T> client, ViessmannAuthHttpClient authClient)
            : base(new ViessmannAuth<T>(tokenStore, config.Value.Username, config.Value.Password))
        {
            VitotrolDeviceId = config.Value.VitotrolDeviceId;
            VitotrolInstallationId = config.Value.VitotrolInstallationId;

            PlattformInstallationId = config.Value.PlattformInstallationId;
            PlattformGatewayId = config.Value.PlattformGatewayId;
            PlattformApiClientId = config.Value.PlattformApiClientId;
            PlattformApiClientSecret = config.Value.PlattformApiClientSecret;

            Client = client.Client;
            AuthClient = authClient.AuthClient;
        }
    }
}