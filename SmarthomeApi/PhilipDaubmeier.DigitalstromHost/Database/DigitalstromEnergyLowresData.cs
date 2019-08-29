using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromEnergyLowresData : IDigitalstromEnergyMidLowresData
    {
        private int Interval30minThisMonth => (int)Math.Floor((Key.AddMonths(1) - Key).TotalDays * 24 * 2);

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(34)]
        public string CircuitId { get; set; }

        [ForeignKey("CircuitId")]
        public DigitalstromCircuit Circuit { get; set; }

        [Required, Column("Month")]
        public DateTime Key { get; set; }

        [MaxLength(4000)]
        public string EnergyCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> EnergySeries
        {
            get => EnergyCurve.ToTimeseries<int>(new TimeSeriesSpan(Key, Key.AddMonths(1), Interval30minThisMonth));
            set { EnergyCurve = value.ToBase64(); }
        }
    }
}