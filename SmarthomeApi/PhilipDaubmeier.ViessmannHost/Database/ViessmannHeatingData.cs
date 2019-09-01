using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.ViessmannHost.Database
{
    public class ViessmannHeatingData : TimeSeriesDbEntityBase
    {
        [NotMapped]
        protected override TimeSeriesSpan Span => new TimeSeriesSpan(Key, Key.AddDays(1), TimeSeriesSpan.Spacing.Spacing5Min);

        [NotMapped]
        protected override int DecimalPlaces => 1;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, Column("Day")]
        public override DateTime Key { get; set; }

        [Required]
        public double BurnerHoursTotal { get; set; }

        [Required]
        public int BurnerStartsTotal { get; set; }

        [MaxLength(800)]
        public string BurnerMinutesCurve { get; set; }

        [MaxLength(800)]
        public string BurnerStartsCurve { get; set; }

        [MaxLength(800)]
        public string BurnerModulationCurve { get; set; }

        [MaxLength(800)]
        public string OutsideTempCurve { get; set; }

        [MaxLength(800)]
        public string BoilerTempCurve { get; set; }

        [MaxLength(800)]
        public string BoilerTempMainCurve { get; set; }

        [MaxLength(800)]
        public string Circuit0TempCurve { get; set; }

        [MaxLength(800)]
        public string Circuit1TempCurve { get; set; }

        [MaxLength(800)]
        public string DhwTempCurve { get; set; }
        
        [MaxLength(100)]
        public string BurnerActiveCurve { get; set; }

        [MaxLength(100)]
        public string Circuit0PumpCurve { get; set; }

        [MaxLength(100)]
        public string Circuit1PumpCurve { get; set; }

        [MaxLength(100)]
        public string DhwPrimaryPumpCurve { get; set; }

        [MaxLength(100)]
        public string DhwCirculationPumpCurve { get; set; }

        [NotMapped]
        public TimeSeries<double> BurnerMinutesSeries => BurnerMinutesCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<int> BurnerStartsSeries => BurnerStartsCurve.ToTimeseries<int>(Span);

        [NotMapped]
        public TimeSeries<int> BurnerModulationSeries => BurnerModulationCurve.ToTimeseries<int>(Span);
        
        [NotMapped]
        public TimeSeries<double> OutsideTempSeries => OutsideTempCurve.ToTimeseries<double>(Span);
        
        [NotMapped]
        public TimeSeries<double> BoilerTempSeries => BoilerTempCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<double> BoilerTempMainSeries => BoilerTempMainCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<double> Circuit0TempSeries => Circuit0TempCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<double> Circuit1TempSeries => Circuit1TempCurve.ToTimeseries<double>(Span);

        [NotMapped]
        public TimeSeries<double> DhwTempSeries => DhwTempCurve.ToTimeseries<double>(Span);
        
        [NotMapped]
        public TimeSeries<bool> BurnerActiveSeries => BurnerActiveCurve.ToTimeseries<bool>(Span);

        [NotMapped]
        public TimeSeries<bool> Circuit0PumpSeries => Circuit0PumpCurve.ToTimeseries<bool>(Span);

        [NotMapped]
        public TimeSeries<bool> Circuit1PumpSeries => Circuit1PumpCurve.ToTimeseries<bool>(Span);

        [NotMapped]
        public TimeSeries<bool> DhwPrimaryPumpSeries => DhwPrimaryPumpCurve.ToTimeseries<bool>(Span);

        [NotMapped]
        public TimeSeries<bool> DhwCirculationPumpSeries => DhwCirculationPumpCurve.ToTimeseries<bool>(Span);
    }
}