using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromZoneSensorData : TimeSeriesDbEntityBase
    {
        [NotMapped]
        protected override TimeSeriesSpan Span => new TimeSeriesSpan(Key, Key.AddDays(1), TimeSeriesSpan.Spacing.Spacing5Min);

        [NotMapped]
        protected override int DecimalPlaces => 2;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int ZoneId { get; set; }

        [ForeignKey("ZoneId")]
        public DigitalstromZone Zone { get; set; }

        [Required, Column("Day")]
        public override DateTime Key { get; set; }

        [MaxLength(800)]
        public string TemperatureCurve { get; set; }

        [MaxLength(800)]
        public string HumidityCurve { get; set; }

        [NotMapped]
        public TimeSeries<double> TemperatureSeries => TemperatureCurve.ToTimeseries<double>(Span, DecimalPlaces);

        [NotMapped]
        public TimeSeries<double> HumiditySeries => HumidityCurve.ToTimeseries<double>(Span, DecimalPlaces);
    }
}