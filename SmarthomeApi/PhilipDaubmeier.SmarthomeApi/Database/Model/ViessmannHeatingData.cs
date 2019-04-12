using PhilipDaubmeier.CompactTimeSeries;
using SmarthomeApi.FormatParsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class ViessmannHeatingData
    {
        private const int interval5min = 60 / 5 * 24;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime Day { get; set; }

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
        public TimeSeries<double> BurnerMinutesSeries
        {
            get => BurnerMinutesCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { BurnerMinutesCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> BurnerStartsSeries
        {
            get => BurnerStartsCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { BurnerStartsCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> BurnerModulationSeries
        {
            get => BurnerModulationCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { BurnerModulationCurve = value.ToBase64(); }
        }
        
        [NotMapped]
        public TimeSeries<double> OutsideTempSeries
        {
            get => OutsideTempCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { OutsideTempCurve = value.ToBase64(); }
        }
        
        [NotMapped]
        public TimeSeries<double> BoilerTempSeries
        {
            get => BoilerTempCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { BoilerTempCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<double> BoilerTempMainSeries
        {
            get => BoilerTempMainCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { BoilerTempMainCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<double> Circuit0TempSeries
        {
            get => Circuit0TempCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { Circuit0TempCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<double> Circuit1TempSeries
        {
            get => Circuit1TempCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { Circuit1TempCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<double> DhwTempSeries
        {
            get => DhwTempCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { DhwTempCurve = value.ToBase64(); }
        }
        
        [NotMapped]
        public TimeSeries<bool> BurnerActiveSeries
        {
            get => BurnerActiveCurve.ToTimeseries<bool>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { BurnerActiveCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<bool> Circuit0PumpSeries
        {
            get => Circuit0PumpCurve.ToTimeseries<bool>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { Circuit0PumpCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<bool> Circuit1PumpSeries
        {
            get => Circuit1PumpCurve.ToTimeseries<bool>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { Circuit1PumpCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<bool> DhwPrimaryPumpSeries
        {
            get => DhwPrimaryPumpCurve.ToTimeseries<bool>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { DhwPrimaryPumpCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<bool> DhwCirculationPumpSeries
        {
            get => DhwCirculationPumpCurve.ToTimeseries<bool>(new TimeSeriesSpan(Day, Day.AddDays(1), interval5min));
            set { DhwCirculationPumpCurve = value.ToBase64(); }
        }
    }
}