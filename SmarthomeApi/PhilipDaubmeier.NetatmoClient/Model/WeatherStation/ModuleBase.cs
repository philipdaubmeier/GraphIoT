using Newtonsoft.Json;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class ModuleBase
    {
        [JsonProperty("_id")]
        public ModuleId Id { get; set; }
        public string ModuleName { get; set; }
        public string Type { get; set; }
        public int Firmware { get; set; }
        public ModuleData DashboardData { get; set; }
        public List<Measure> DataType { get; set; }
    }
}