﻿using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation.WeatherStation
{
    public class ModuleData
    {
        public int TimeUtc { get; set; }

        public double? Temperature { get; set; }
        public string TempTrend { get; set; }
        public int? DateMaxTemp { get; set; }
        public int? DateMinTemp { get; set; }
        public double? MinTemp { get; set; }
        public double? MaxTemp { get; set; }

        public int? Humidity { get; set; }
        public int? CO2 { get; set; }
        public double? Pressure { get; set; }
        public double? AbsolutePressure { get; set; }
        public int? Noise { get; set; }

        public double Rain { get; set; }

        public int? WindAngle { get; set; }
        public int? WindStrength { get; set; }
        public int? GustAngle { get; set; }
        public int? GustStrength { get; set; }
        public List<WindHistoric> WindHistoric { get; set; }
        public int? DateMaxWindStr { get; set; }
        public int? MaxWindAngle { get; set; }
        public int? MaxWindStr { get; set; }
    }
}