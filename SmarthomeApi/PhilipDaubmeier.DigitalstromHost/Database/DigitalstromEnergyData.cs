using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromEnergyLowresData : DigitalstromEnergyData
    {
        protected override TimeSeriesSpan Span => SpanMonth;

        [Required, Column("Month")]
        public override DateTime Key { get; set; }
    }

    public class DigitalstromEnergyMidresData : DigitalstromEnergyData
    {
        protected override TimeSeriesSpan Span => SpanDay;

        [Required, Column("Day")]
        public override DateTime Key { get; set; }
    }

    public abstract class DigitalstromEnergyData : TimeSeriesDbEntityBase
    {
        [NotMapped]
        protected override int DecimalPlaces => 1;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(34)]
        public string CircuitId { get; set; }

        [ForeignKey("CircuitId")]
        public DigitalstromCircuit Circuit { get; set; }

        [MaxLength(4000)]
        public string EnergyCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> EnergySeries => EnergyCurve.ToTimeseries<int>(Span);
    }
}