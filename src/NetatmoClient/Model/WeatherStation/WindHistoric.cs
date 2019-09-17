using Newtonsoft.Json;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class WindHistoric
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime TimeUtc { get; set; }

        public int WindStrength { get; set; }
        public int WindAngle { get; set; }
    }
}