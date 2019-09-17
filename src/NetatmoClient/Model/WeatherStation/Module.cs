using Newtonsoft.Json;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class Module : ModuleBase
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastMessage { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastSeen { get; set; }

        public int RfStatus { get; set; }
        public int BatteryPercent { get; set; }
        public int BatteryVp { get; set; }
    }
}