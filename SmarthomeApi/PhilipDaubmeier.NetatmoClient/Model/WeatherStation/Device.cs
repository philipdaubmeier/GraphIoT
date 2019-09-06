using Newtonsoft.Json;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation.WeatherStation
{
    public class Device
    {
        [JsonProperty("_id")]
        public ModuleId Id { get; set; }
        public bool Co2Calibrating { get; set; }
        public int Firmware { get; set; }
        public int LastStatusStore { get; set; }
        public int LastUpgrade { get; set; }
        public string ModuleName { get; set; }
        public List<Module> Modules { get; set; }
        public Place Place { get; set; }
        public string StationName { get; set; }
        public string Type { get; set; }
        public int WifiStatus { get; set; }
        public ModuleData DashboardData { get; set; }
        public List<string> DataType { get; set; }
    }
}