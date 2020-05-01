using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.WeConnect.Database
{
    public class WeConnectLowresData : WeConnectData
    {
        public override TimeSeriesSpan Span => SpanMonth160Min;

        [Required, Column("Month")]
        public override DateTime Key { get; set; }
    }

    public class WeConnectMidresData : WeConnectData
    {
        public override TimeSeriesSpan Span => SpanDay5Min;

        [Required, Column("Day")]
        public override DateTime Key { get; set; }

        public int? Mileage { get; set; }
    }

    public abstract class WeConnectData : TimeSeriesDbEntityBase
    {
        protected override int DecimalPlaces => 2;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(20)]
        public string Vin { get; set; } = string.Empty;

        [MaxLength(800)]
        public string? DrivenKilometersCurve { get; set; }

        [MaxLength(800)]
        public string? BatterySocCurve { get; set; }

        [MaxLength(800)]
        public string? ConsumedKwhCurve { get; set; }

        [MaxLength(800)]
        public string? AverageConsumptionCurve { get; set; }

        [MaxLength(800)]
        public string? AverageSpeedCurve { get; set; }

        [MaxLength(100)]
        public string? ChargingStateCurve { get; set; }

        [MaxLength(800)]
        public string? ClimateTempCurve { get; set; }

        [MaxLength(100)]
        public string? ClimateStateCurve { get; set; }

        [MaxLength(100)]
        public string? WindowMeltStateCurve { get; set; }

        [MaxLength(100)]
        public string? RemoteHeatingStateCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> DrivenKilometersSeries => DrivenKilometersCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> BatterySocSeries => BatterySocCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<double> ConsumedKwhSeries => ConsumedKwhCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<double> AverageConsumptionSeries => AverageConsumptionCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<double> AverageSpeedSeries => AverageSpeedCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<bool> ChargingStateSeries => ChargingStateCurve.ToTimeseries<bool>(Span);

        [NotMapped]
        public TimeSeries<double> ClimateTempSeries => ClimateTempCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<bool> ClimateStateSeries => ClimateStateCurve.ToTimeseries<bool>(Span);

        [NotMapped]
        public TimeSeries<bool> WindowMeltStateSeries => WindowMeltStateCurve.ToTimeseries<bool>(Span, DecimalPlaces);

        [NotMapped]
        public TimeSeries<bool> RemoteHeatingStateSeries => RemoteHeatingStateCurve.ToTimeseries<bool>(Span, DecimalPlaces);
    }
}