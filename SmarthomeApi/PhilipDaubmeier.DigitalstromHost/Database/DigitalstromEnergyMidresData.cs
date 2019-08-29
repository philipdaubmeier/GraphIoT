using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromEnergyMidresData : IDigitalstromEnergyMidLowresData
    {
        private const int interval1min = 60 * 24;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(34)]
        public string CircuitId { get; set; }

        [ForeignKey("CircuitId")]
        public DigitalstromCircuit Circuit { get; set; }

        [Required, Column("Day")]
        public DateTime Key { get; set; }

        [MaxLength(4000)]
        public string EnergyCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> EnergySeries
        {
            get => EnergyCurve.ToTimeseries<int>(new TimeSeriesSpan(Key, Key.AddDays(1), interval1min));
            set { EnergyCurve = value.ToBase64(); }
        }
    }
}