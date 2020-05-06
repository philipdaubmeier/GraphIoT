using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Database
{
    public class DigitalstromZoneSensorLowresData : DigitalstromZoneSensorData
    {
        public override TimeSeriesSpan Span => SpanMonth160Min;

        [Required, Column("Month")]
        public override DateTime Key { get; set; }
    }

    public class DigitalstromZoneSensorMidresData : DigitalstromZoneSensorData
    {
        public override TimeSeriesSpan Span => SpanDay5Min;

        [Required, Column("Day")]
        public override DateTime Key { get; set; }
    }

    public abstract class DigitalstromZoneSensorData : TimeSeriesDbEntityBase
    {
        protected override int DecimalPlaces => 2;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int ZoneId { get; set; }

        [ForeignKey("ZoneId")]
        public DigitalstromZone Zone { get; set; } = null!;

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? TemperatureCurve { get; set; }

        [MaxLength(800)]
        [TimeSeries(typeof(double))]
        public string? HumidityCurve { get; set; }

        [NotMapped]
        public TimeSeries<double> TemperatureSeries => TemperatureCurve.ToTimeseries<double>(Span, DecimalPlaces);

        [NotMapped]
        public TimeSeries<double> HumiditySeries => HumidityCurve.ToTimeseries<double>(Span, DecimalPlaces);
    }
}