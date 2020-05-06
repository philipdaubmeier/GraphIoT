using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Database
{
    public class SonnenEnergyLowresData : SonnenEnergyData
    {
        public override TimeSeriesSpan Span => SpanMonth30Min;

        [Required, Column("Month")]
        public override DateTime Key { get; set; }
    }

    public class SonnenEnergyMidresData : SonnenEnergyData
    {
        public override TimeSeriesSpan Span => SpanDay1Min;

        [Required, Column("Day")]
        public override DateTime Key { get; set; }
    }

    public abstract class SonnenEnergyData : TimeSeriesDbEntityBase
    {
        protected override int DecimalPlaces => 2;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(int))]
        public string? ProductionPowerCurve { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(int))]
        public string? ConsumptionPowerCurve { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(int))]
        public string? DirectUsagePowerCurve { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(int))]
        public string? BatteryChargingCurve { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(int))]
        public string? BatteryDischargingCurve { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(int))]
        public string? GridFeedinCurve { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(int))]
        public string? GridPurchaseCurve { get; set; }

        [MaxLength(4000)]
        [TimeSeries(typeof(double))]
        public string? BatteryUsocCurve { get; set; }
    }
}