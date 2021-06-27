using PhilipDaubmeier.ViessmannClient.Model.Devices;
using PhilipDaubmeier.ViessmannClient.Model.Features;
using PhilipDaubmeier.ViessmannClient.Model.Gateways;
using PhilipDaubmeier.ViessmannClient.Model.Installations;
using PhilipDaubmeier.ViessmannClient.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient
{
    public class ViessmannPlatformClient : ViessmannAuthBase
    {
        private const string _baseUri = "https://api.viessmann.com/";

        public ViessmannPlatformClient(IViessmannConnectionProvider<ViessmannPlatformClient> connectionProvider)
            : base(connectionProvider) { }

        public async Task<IEnumerable<Installation>> GetInstallations()
        {
            var uri = $"{_baseUri}iot/v1/equipment/installations";
            return await CallViessmannApi<InstallationResponse, List<Installation>>(new Uri(uri));
        }

        public async Task<IEnumerable<Gateway>> GetGateways(long installationId)
        {
            var uri = $"{_baseUri}iot/v1/equipment/installations/{installationId}/gateways";
            return await CallViessmannApi<GatewayResponse, List<Gateway>>(new Uri(uri));
        }

        public async Task<IEnumerable<Device>> GetDevices(long installationId, long gatewayId)
        {
            var uri = $"{_baseUri}iot/v1/equipment/installations/{installationId}/gateways/{gatewayId}/devices";
            return await CallViessmannApi<DeviceResponse, List<Device>>(new Uri(uri));
        }

        public async Task<GatewayFeatureList> GetGatewayFeatures(long installationId, long gatewayId)
        {
            var uri = $"{_baseUri}iot/v1/equipment/installations/{installationId}/gateways/{gatewayId}/features";
            return await CallViessmannApi<FeatureResponse<GatewayFeatureList>, GatewayFeatureList>(new Uri(uri));
        }

        public async Task<DeviceFeatureList> GetDeviceFeatures(long installationId, long gatewayId, long deviceId = 0)
        {
            var uri = $"{_baseUri}iot/v1/equipment/installations/{installationId}/gateways/{gatewayId}/devices/{deviceId}/features";
            return await CallViessmannApi<FeatureResponse<DeviceFeatureList>, DeviceFeatureList>(new Uri(uri));
        }
    }
}