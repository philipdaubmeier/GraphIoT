using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class ModuleData
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime TimeUtc { get; set; }

        public double? Temperature { get; set; }
        public string TempTrend { get; set; } = string.Empty;

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime DateMaxTemp { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime DateMinTemp { get; set; }

        public double? MinTemp { get; set; }
        public double? MaxTemp { get; set; }

        public int? Humidity { get; set; }

        [JsonProperty("CO2")]
        public int? CO2 { get; set; }
        public double? Pressure { get; set; }

        [JsonProperty("AbsolutePressure")]
        public double? AbsolutePressure { get; set; }
        public int? Noise { get; set; }

        public double? Rain { get; set; }

        [JsonProperty("sum_rain_1")]
        public double? SumRain1 { get; set; }

        [JsonProperty("sum_rain_24")]
        public double? SumRain24 { get; set; }

        public int? WindAngle { get; set; }
        public int? WindStrength { get; set; }
        public int? GustAngle { get; set; }
        public int? GustStrength { get; set; }
        public List<WindHistoric> WindHistoric { get; set; } = new List<WindHistoric>();
        public int? DateMaxWindStr { get; set; }
        public int? MaxWindAngle { get; set; }
        public int? MaxWindStr { get; set; }
    }
}