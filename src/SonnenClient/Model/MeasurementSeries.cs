using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class MeasurementSeries
    {
        public string MeasurementMethod { get; set; } = string.Empty;
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;
        public Resolution Resolution { get; set; } = "1s";
        public List<int?> ProductionPower { get; set; } = new List<int?>();
        public List<int?> ConsumptionPower { get; set; } = new List<int?>();
        public List<int?> DirectUsagePower { get; set; } = new List<int?>();
        public List<int?> BatteryCharging { get; set; } = new List<int?>();
        public List<int?> BatteryDischarging { get; set; } = new List<int?>();
        public List<int?> GridFeedin { get; set; } = new List<int?>();
        public List<int?> GridPurchase { get; set; } = new List<int?>();
        public List<double?> BatteryUsoc { get; set; } = new List<double?>();
    }
}