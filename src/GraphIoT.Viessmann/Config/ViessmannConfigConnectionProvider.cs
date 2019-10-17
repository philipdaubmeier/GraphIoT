using Microsoft.Extensions.Options;
using PhilipDaubmeier.TokenStore;
using PhilipDaubmeier.ViessmannClient.Model;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannAuthClientProvider
    {
        public HttpClient AuthClient { get; private set; }

        public ViessmannAuthClientProvider(HttpClient authClient) => AuthClient = authClient;
    }

    public class ViessmannConfigConnectionProvider<T> : ViessmannConnectionProvider<T>
    {
        public ViessmannConfigConnectionProvider(TokenStore<T> tokenStore, IOptions<ViessmannConfig> config, ViessmannAuthClientProvider authClient, HttpClient client)
        {
            VitotrolDeviceId = config.Value.VitotrolDeviceId;
            VitotrolInstallationId = config.Value.VitotrolInstallationId;

            PlattformInstallationId = config.Value.PlattformInstallationId;
            PlattformGatewayId = config.Value.PlattformGatewayId;
            PlattformApiClientId = config.Value.PlattformApiClientId;
            PlattformApiClientSecret = config.Value.PlattformApiClientSecret;

            AuthData = new ViessmannAuth<T>(tokenStore, config.Value.Username, config.Value.Password);

            Client = client;
            AuthClient = authClient.AuthClient;
        }
    }
}