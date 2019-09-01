using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromEnergyLowresData : TimeSeriesDbEntityBase, IDigitalstromEnergyMidLowresData
    {
        [NotMapped]
        protected override TimeSeriesSpan Span => new TimeSeriesSpan(Key, Key.AddMonths(1), (int)Math.Floor((Key.AddMonths(1) - Key).TotalDays * 24 * 2));

        [NotMapped]
        protected override int DecimalPlaces => 1;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(34)]
        public string CircuitId { get; set; }

        [ForeignKey("CircuitId")]
        public DigitalstromCircuit Circuit { get; set; }

        [Required, Column("Month")]
        public override DateTime Key { get; set; }

        [MaxLength(4000)]
        public string EnergyCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> EnergySeries => EnergyCurve.ToTimeseries<int>(Span);
    }
}