using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromEnergyHighresData
    {
        private static TimeSeriesSpan Span(DateTime day) => new TimeSeriesSpan(day, TimeSeriesSpan.Spacing.Spacing1Sec, (int)TimeSeriesSpan.Spacing.Spacing1Day);

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        [Required, Column("Day")]
        public DateTime Key { get; set; }

        public byte[] EnergyCurvesEveryMeter { get; set; }

        [NotMapped]
        public TimeSeriesStreamCollection<Dsuid, int> EnergySeriesEveryMeter
        {
            get => new TimeSeriesStreamCollection<Dsuid, int>(EnergyCurvesEveryMeter, Dsuid.Size, stream => Dsuid.ReadFrom(stream), Span(Key));
            set { EnergyCurvesEveryMeter = value.ToCompressedByteArray(); }
        }

        public static TimeSeriesStreamCollection<Dsuid, int> InitialEnergySeriesEveryMeter(DateTime day, IEnumerable<Dsuid> dsuids)
        {
            return new TimeSeriesStreamCollection<Dsuid, int>(dsuids, Dsuid.Size, (dsuid, stream) => dsuid.WriteTo(stream), Span(day));
        }
    }
}