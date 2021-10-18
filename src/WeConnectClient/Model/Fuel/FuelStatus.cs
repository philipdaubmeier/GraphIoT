using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.WeConnectClient.Model.Fuel
{
    public class FuelStatus
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CarCapturedTimestamp { get; set; } = DateTime.MinValue;

        public string EngineType => Properties.FirstOrDefault(x => x.Name == "engineType")?.Value ?? string.Empty;
        public double? RemainingRangeKm => double.TryParse(Properties.FirstOrDefault(x => x.Name == "remainingRange_km")?.Value, out double val) ? val : null;
        public double? CurrentFuelLevelPercent => double.TryParse(Properties.FirstOrDefault(x => x.Name == "currentFuelLevel_pct")?.Value, out double val) ? val : null;
        public double? CurrentSocPercent => double.TryParse(Properties.FirstOrDefault(x => x.Name == "currentSOC_pct")?.Value, out double val) ? val : null;

        public List<FuelStatusProperty> Properties { get; set; } = new();
    }

    public class FuelStatusProperty
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}