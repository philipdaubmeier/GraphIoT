using Newtonsoft.Json;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class Camera
    {
        public ModuleId Id { get; set; } = string.Empty;

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastSetup { get; set; }

        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string VpnUrl { get; set; } = string.Empty;
        public bool IsLocal { get; set; }
        public string SdStatus { get; set; } = string.Empty;
        public string AlimStatus { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LightModeStatus { get; set; } = string.Empty;
    }
}