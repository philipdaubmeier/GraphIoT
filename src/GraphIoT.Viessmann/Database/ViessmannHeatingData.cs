using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Database
{
    public class ViessmannHeatingLowresData : ViessmannHeatingData
    {
        public override TimeSeriesSpan Span => SpanMonth160Min;

        [Required, Column("Month")]
        public override DateTime Key { get; set; }
    }

    public class ViessmannHeatingMidresData : ViessmannHeatingData
    {
        public override TimeSeriesSpan Span => SpanDay5Min;

        [Required, Column("Day")]
        public override DateTime Key { get; set; }

        [Required]
        public double BurnerHoursTotal { get; set; } = 0d;

        [Required]
        public int BurnerStartsTotal { get; set; } = 0;
    }

    public abstract class ViessmannHeatingData : TimeSeriesDbEntityBase
    {
        protected override int DecimalPlaces => 1;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? BurnerMinutesCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(int))]
        public string? BurnerStartsCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(int))]
        public string? BurnerModulationCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? OutsideTempCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? BoilerTempCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? BoilerTempMainCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? Circuit0TempCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? Circuit1TempCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? DhwTempCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? BurnerActiveCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? Circuit0PumpCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? Circuit1PumpCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? DhwPrimaryPumpCurve { get; set; }

        [MaxLength(100)]
        [TimeSeries(typeof(bool))]
        public string? DhwCirculationPumpCurve { get; set; }

        [NotMapped]
        public TimeSeries<double> BurnerMinutesSeries => BurnerMinutesCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<int> BurnerStartsSeries => BurnerStartsCurve.ToTimeseries<int>(Span);
    }
}