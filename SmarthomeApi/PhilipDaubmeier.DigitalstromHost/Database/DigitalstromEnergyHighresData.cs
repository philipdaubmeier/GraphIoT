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
        
        [Required]
        public DateTime Day { get; set; }
        
        public byte[] EnergyCurvesEveryMeter { get; set; }

        [NotMapped]
        public TimeSeriesStreamCollection<DSUID, int> EnergySeriesEveryMeter
        {
            get => new TimeSeriesStreamCollection<DSUID, int>(EnergyCurvesEveryMeter, DSUID.Size, stream => DSUID.ReadFrom(stream), Span(Day));
            set { EnergyCurvesEveryMeter = value.ToCompressedByteArray(); }
        }

        public static TimeSeriesStreamCollection<DSUID, int> InitialEnergySeriesEveryMeter(DateTime day, IEnumerable<DSUID> dsuids)
        {
            return new TimeSeriesStreamCollection<DSUID, int>(dsuids, DSUID.Size, (dsuid, stream) => dsuid.WriteTo(stream), Span(day));
        }
    }
}