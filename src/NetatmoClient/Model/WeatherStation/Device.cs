using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class Device : ModuleBase
    {
        public bool Co2Calibrating { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime DateSetup { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastStatusStore { get; set; }

        public List<Module> Modules { get; set; } = new List<Module>();
        public Place Place { get; set; } = new Place();
        public string StationName { get; set; } = string.Empty;
        public int WifiStatus { get; set; }
    }
}