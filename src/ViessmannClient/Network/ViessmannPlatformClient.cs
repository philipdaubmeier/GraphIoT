using PhilipDaubmeier.ViessmannClient.Model;
using PhilipDaubmeier.ViessmannClient.Model.Features;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient
{
    public class ViessmannPlatformClient : ViessmannAuthBase
    {
        public ViessmannPlatformClient(IViessmannConnectionProvider<ViessmannPlatformClient> connectionProvider)
            : base(connectionProvider) { }

        public async Task<string> GetInstallations()
        {
            var uri = new Uri("https://api.viessmann-platform.io/general-management/v1/installations?expanded=true");
            return await (await RequestViessmannApi(uri)).Content.ReadAsStringAsync();
        }

        public async Task<FeatureList> GetFeatures()
        {
            var uri = $"https://api.viessmann-platform.io/operational-data/v2/installations/{_connectionProvider.PlattformInstallationId}/gateways/{_connectionProvider.PlattformGatewayId}/devices/0/features?reduceHypermedia=true";
            return await CallViessmannApi<FeatureList>(new Uri(uri));
        }
    }
}