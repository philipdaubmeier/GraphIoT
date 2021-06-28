using PhilipDaubmeier.ViessmannClient.Model.Devices;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class GatewayFeatureList : FeatureList
    {
        public bool IsGatewayStatusOnline()
        {
            return GetProperties(FeatureName.Name.GatewayStatus)?.Online?.Value ?? false;
        }

        public decimal GetGatewayWifiStrength()
        {
            return GetProperties(FeatureName.Name.GatewayWifi)?.Strength?.Value ?? 0m;
        }

        public int GetGatewayWifiChannel()
        {
            return GetProperties(FeatureName.Name.GatewayWifi)?.Channel?.Value ?? 0;
        }

        public bool IsGatewayBmuconnectionOk()
        {
            return GetProperties(FeatureName.Name.GatewayBmuconnection)?.Status?.Value?.Equals("ok", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public IEnumerable<Device> GetGatewayDevices()
        {
            return GetProperties(FeatureName.Name.GatewayDevices)?.Devices?.Value ?? new List<Device>();
        }

        public string GetGatewayFirmwareVersion()
        {
            return GetProperties(FeatureName.Name.GatewayFirmware)?.Version?.Value ?? string.Empty;
        }

        public string GetGatewayFirmwareUpdateStatus()
        {
            return GetProperties(FeatureName.Name.GatewayFirmware)?.UpdateStatus?.Value ?? string.Empty;
        }
    }
}