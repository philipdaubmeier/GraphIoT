using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class Device : ModuleBase
    {
        public bool Co2Calibrating { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastStatusStore { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastUpgrade { get; set; }

        public List<Module> Modules { get; set; }
        public Place Place { get; set; }
        public string StationName { get; set; }
        public int WifiStatus { get; set; }
    }
}