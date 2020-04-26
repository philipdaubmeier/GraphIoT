using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class ModuleData
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime TimeUtc { get; set; }

        [JsonPropertyName("Temperature")]
        public double? Temperature { get; set; }
        public string TempTrend { get; set; } = string.Empty;

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime DateMaxTemp { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime DateMinTemp { get; set; }

        public double? MinTemp { get; set; }
        public double? MaxTemp { get; set; }

        [JsonPropertyName("Humidity")]
        public int? Humidity { get; set; }

        [JsonPropertyName("CO2")]
        public int? CO2 { get; set; }

        [JsonPropertyName("Pressure")]
        public double? Pressure { get; set; }

        [JsonPropertyName("AbsolutePressure")]
        public double? AbsolutePressure { get; set; }

        [JsonPropertyName("Noise")]
        public int? Noise { get; set; }

        [JsonPropertyName("Rain")]
        public double? Rain { get; set; }

        [JsonPropertyName("sum_rain_1")]
        public double? SumRain1 { get; set; }

        [JsonPropertyName("sum_rain_24")]
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