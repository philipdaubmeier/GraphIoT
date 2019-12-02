using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class StatisticsSeries
    {
        public string? MeasurementMethod { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public Resolution? Resolution { get; set; }
        public List<int?>? ConsumedEnergy { get; set; }
        public List<int?>? DirectUsageEnergy { get; set; }
        public List<int?>? ProducedEnergy { get; set; }
        public List<int?>? BatteryDischargedEnergy { get; set; }
        public List<int?>? BatteryChargedEnergy { get; set; }
        public List<int?>? BatteryChargedFromSunEnergy { get; set; }
        public List<int?>? BatteryChargedFromGridEnergy { get; set; }
        public List<int?>? GridPurchaseEnergy { get; set; }
        public List<int?>? GridFeedinEnergy { get; set; }
    }
}