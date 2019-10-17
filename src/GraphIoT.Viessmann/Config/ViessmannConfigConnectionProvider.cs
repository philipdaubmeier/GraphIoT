using Microsoft.Extensions.Options;
using PhilipDaubmeier.TokenStore;
using PhilipDaubmeier.ViessmannClient.Model;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannConfigConnectionProvider<T> : IViessmannConnectionProvider<T>
    {
        public IViessmannAuth AuthData { get; private set; }
        public HttpClient Client { get; private set; }
        public HttpClient AuthClient { get; private set; }

        public string VitotrolDeviceId { get; private set; }
        public string VitotrolInstallationId { get; private set; }

        public string PlattformInstallationId { get; private set; }
        public string PlattformGatewayId { get; private set; }
        public string PlattformApiClientId { get; private set; }
        public string PlattformApiClientSecret { get; private set; }

        public ViessmannConfigConnectionProvider(TokenStore<T> tokenStore, IOptions<ViessmannConfig> config)
        {
            VitotrolDeviceId = config.Value.VitotrolDeviceId;
            VitotrolInstallationId = config.Value.VitotrolInstallationId;

            PlattformInstallationId = config.Value.PlattformInstallationId;
            PlattformGatewayId = config.Value.PlattformGatewayId;
            PlattformApiClientId = config.Value.PlattformApiClientId;
            PlattformApiClientSecret = config.Value.PlattformApiClientSecret;

            AuthData = new ViessmannAuth<T>(tokenStore, config.Value.Username, config.Value.Password);
        }
    }
}