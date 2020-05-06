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
        [TimeSeries(typeof(int))]
        public string? DrivenKilometersCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(int))]
        public string? BatterySocCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? TripLengthKmCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? TripDurationCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? TripAverageSpeedCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? TripConsumedKwhCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? TripAverageConsumptionCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? ChargingStateCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? ClimateTempCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? ClimateStateCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? WindowMeltStateCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? RemoteHeatingStateCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> DrivenKilometersSeries => DrivenKilometersCurve.ToTimeseries<int>(Span);
    }
}