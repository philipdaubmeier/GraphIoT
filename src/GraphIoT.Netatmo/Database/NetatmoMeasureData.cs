using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Database
{
    public class NetatmoMeasureLowresData : NetatmoMeasureData
    {
        public override TimeSeriesSpan Span => SpanMonth160Min;

        [Required, Column("Month")]
        public override DateTime Key { get; set; }
    }

    public class NetatmoMeasureMidresData : NetatmoMeasureData
    {
        public override TimeSeriesSpan Span => SpanDay5Min;

        [Required, Column("Day")]
        public override DateTime Key { get; set; }
    }

    public abstract class NetatmoMeasureData : TimeSeriesDbEntityBase
    {
        protected override int DecimalPlaces => Math.Min(5, Math.Max(0, Decimals));

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid ModuleMeasureId { get; set; }

        [ForeignKey("ModuleMeasureId")]
        public NetatmoModuleMeasure ModuleMeasure { get; set; } = null!;

        [Required]
        public int Decimals { get; set; }

        [MaxLength(800)]
        public string? MeasureCurve { get; set; }

        [NotMapped]
        public TimeSeries<double> MeasureSeries => MeasureCurve.ToTimeseries<double>(Span, DecimalPlaces);
    }
}