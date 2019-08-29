using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.ViessmannHost.Database
{
    public class ViessmannSolarData : ITimeSeriesDbEntity
    {
        private const int interval5min = 60 / 5 * 24;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, Column("Day")]
        public DateTime Key { get; set; }

        public int? SolarWhTotal { get; set; }

        [MaxLength(800)]
        public string SolarWhCurve { get; set; }

        [MaxLength(800)]
        public string SolarCollectorTempCurve { get; set; }

        [MaxLength(800)]
        public string SolarHotwaterTempCurve { get; set; }

        [MaxLength(100)]
        public string SolarPumpStateCurve { get; set; }

        [MaxLength(100)]
        public string SolarSuppressionCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> SolarWhSeries => ToSeries<int>(SolarWhCurve);

        [NotMapped]
        public TimeSeries<double> SolarCollectorTempSeries => ToSeries<double>(SolarCollectorTempCurve);

        [NotMapped]
        public TimeSeries<double> SolarHotwaterTempSeries => ToSeries<double>(SolarHotwaterTempCurve);

        [NotMapped]
        public TimeSeries<bool> SolarPumpStateSeries => ToSeries<bool>(SolarPumpStateCurve);

        [NotMapped]
        public TimeSeries<bool> SolarSuppressionSeries => ToSeries<bool>(SolarSuppressionCurve);

        public TimeSeries<T> GetSeries<T>(int index) where T : struct
        {
            switch (index)
            {
                case 0: return ToSeries<T>(SolarWhCurve);
                case 1: return ToSeries<T>(SolarCollectorTempCurve);
                case 2: return ToSeries<T>(SolarHotwaterTempCurve);
                case 3: return ToSeries<T>(SolarPumpStateCurve);
                case 4: return ToSeries<T>(SolarSuppressionCurve);
                default: return null;
            }
        }

        private TimeSeries<T> ToSeries<T>(string curve) where T : struct => curve.ToTimeseries<T>(new TimeSeriesSpan(Key, Key.AddDays(1), interval5min));

        public void SetSeries<T>(int index, TimeSeries<T> series) where T : struct
        {
            switch (index)
            {
                case 0: SolarWhCurve = series.ToBase64(); break;
                case 1: SolarCollectorTempCurve = series.ToBase64(); break;
                case 2: SolarHotwaterTempCurve = series.ToBase64(); break;
                case 3: SolarPumpStateCurve = series.ToBase64(); break;
                case 4: SolarSuppressionCurve = series.ToBase64(); break;
            }
        }
    }
}