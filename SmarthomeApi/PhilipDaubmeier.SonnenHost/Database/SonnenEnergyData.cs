using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.SonnenHost.Database
{
    public class SonnenEnergyData : TimeSeriesDbEntityBase
    {
        [NotMapped]
        protected override TimeSeriesSpan Span => new TimeSeriesSpan(Key, Key.AddDays(1), TimeSeriesSpan.Spacing.Spacing1Min);

        [NotMapped]
        protected override int DecimalPlaces => 2;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, Column("Day")]
        public override DateTime Key { get; set; }

        [MaxLength(4000)]
        public string ProductionPowerCurve { get; set; }

        [MaxLength(4000)]
        public string ConsumptionPowerCurve { get; set; }

        [MaxLength(4000)]
        public string DirectUsagePowerCurve { get; set; }

        [MaxLength(4000)]
        public string BatteryChargingCurve { get; set; }

        [MaxLength(4000)]
        public string BatteryDischargingCurve { get; set; }

        [MaxLength(4000)]
        public string GridFeedinCurve { get; set; }

        [MaxLength(4000)]
        public string GridPurchaseCurve { get; set; }

        [MaxLength(4000)]
        public string BatteryUsocCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> ProductionPowerSeries => ProductionPowerCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> ConsumptionPowerSeries => ConsumptionPowerCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> DirectUsagePowerSeries => DirectUsagePowerCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> BatteryChargingSeries => BatteryChargingCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> BatteryDischargingSeries => BatteryDischargingCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> GridFeedinSeries => GridFeedinCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> GridPurchaseSeries => GridPurchaseCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<double> BatteryUsocSeries => BatteryUsocCurve.ToTimeseries<double>(Span, DecimalPlaces);
    }
}