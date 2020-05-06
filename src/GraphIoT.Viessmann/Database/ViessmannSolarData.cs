using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Database
{
    public class ViessmannSolarLowresData : ViessmannSolarData
    {
        public override TimeSeriesSpan Span => SpanMonth160Min;

        [Required, Column("Month")]
        public override DateTime Key { get; set; }
    }

    public class ViessmannSolarMidresData : ViessmannSolarData
    {
        public override TimeSeriesSpan Span => SpanDay5Min;

        [Required, Column("Day")]
        public override DateTime Key { get; set; }

        public int? SolarWhTotal { get; set; }
    }

    public abstract class ViessmannSolarData : TimeSeriesDbEntityBase
    {
        protected override int DecimalPlaces => 1;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(int))]
        public string? SolarWhCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? SolarCollectorTempCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? SolarHotwaterTempCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? SolarPumpStateCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? SolarSuppressionCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> SolarWhSeries => SolarWhCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<double> SolarCollectorTempSeries => SolarCollectorTempCurve.ToTimeseries<double>(Span);
    }
}