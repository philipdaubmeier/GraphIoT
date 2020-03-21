using PhilipDaubmeier.ViessmannClient.Model;
using PhilipDaubmeier.ViessmannClient.Model.Features;
using PhilipDaubmeier.ViessmannClient.Model.Gateways;
using PhilipDaubmeier.ViessmannClient.Model.Installations;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient
{
    public class ViessmannPlatformClient : ViessmannAuthBase
    {
        private const string _baseUri = "https://api.viessmann-platform.io/";

        public ViessmannPlatformClient(IViessmannConnectionProvider<ViessmannPlatformClient> connectionProvider)
            : base(connectionProvider) { }

        public async Task<InstallationList> GetInstallations()
        {
            var uri = $"{_baseUri}iot/v1/equipment/installations";
            return await CallViessmannApi<InstallationList>(new Uri(uri), g => g?.Data != null);
        }

        public async Task<GatewayList> GetGateways(long installationId)
        {
            var uri = $"{_baseUri}iot/v1/equipment/installations/{installationId}/gateways";
            return await CallViessmannApi<GatewayList>(new Uri(uri), g => g?.Data != null);
        }

        public async Task<FeatureList> GetFeatures(long installationId, long gatewayId)
        {
            var uri = $"{_baseUri}operational-data/v2/installations/{installationId}/gateways/{gatewayId}/devices/0/features?reduceHypermedia=true";
            return await CallViessmannApi<FeatureList>(new Uri(uri), f => f?.Features != null);
        }
    }
}