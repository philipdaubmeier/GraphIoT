using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class MeasurementSeries
    {
        public string MeasurementMethod { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Resolution Resolution { get; set; }
        public List<int?> ProductionPower { get; set; }
        public List<int?> ConsumptionPower { get; set; }
        public List<int?> DirectUsagePower { get; set; }
        public List<int?> BatteryCharging { get; set; }
        public List<int?> BatteryDischarging { get; set; }
        public List<int?> GridFeedin { get; set; }
        public List<int?> GridPurchase { get; set; }
        public List<double?> BatteryUsoc { get; set; }
    }
}