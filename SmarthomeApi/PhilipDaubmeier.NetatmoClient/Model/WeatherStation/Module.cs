using Newtonsoft.Json;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation.WeatherStation
{
    public class Module
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public string ModuleName { get; set; }
        public string Type { get; set; }
        public int Firmware { get; set; }
        public int LastMessage { get; set; }
        public int LastSeen { get; set; }
        public int RfStatus { get; set; }
        public int BatteryVp { get; set; }
        public ModuleData DashboardData { get; set; }
        public List<string> DataType { get; set; }
    }
}