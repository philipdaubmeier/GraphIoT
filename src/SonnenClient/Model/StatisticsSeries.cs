using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class StatisticsSeries
    {
        public string MeasurementMethod { get; set; } = string.Empty;
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MinValue;
        public Resolution Resolution { get; set; } = "1s";
        public List<int?> ConsumedEnergy { get; set; } = new List<int?>();
        public List<int?> DirectUsageEnergy { get; set; } = new List<int?>();
        public List<int?> ProducedEnergy { get; set; } = new List<int?>();
        public List<int?> BatteryDischargedEnergy { get; set; } = new List<int?>();
        public List<int?> BatteryChargedEnergy { get; set; } = new List<int?>();
        public List<int?> BatteryChargedFromSunEnergy { get; set; } = new List<int?>();
        public List<int?> BatteryChargedFromGridEnergy { get; set; } = new List<int?>();
        public List<int?> GridPurchaseEnergy { get; set; } = new List<int?>();
        public List<int?> GridFeedinEnergy { get; set; } = new List<int?>();
    }
}