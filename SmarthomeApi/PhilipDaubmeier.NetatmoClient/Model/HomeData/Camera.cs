using PhilipDaubmeier.NetatmoClient.Model.Core;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class Camera
    {
        public ModuleId Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string VpnUrl { get; set; }
        public bool IsLocal { get; set; }
        public string SdStatus { get; set; }
        public string AlimStatus { get; set; }
        public string Name { get; set; }
        public string LightModeStatus { get; set; }
    }
}