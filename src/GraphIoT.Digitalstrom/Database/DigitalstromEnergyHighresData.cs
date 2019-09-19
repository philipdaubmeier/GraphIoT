using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.GraphIoT.Core.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Database
{
    public class DigitalstromEnergyHighresData : TimeSeriesDbEntityBase
    {
        public override TimeSeriesSpan Span => SpanDay1Sec;

        protected override int DecimalPlaces => 1;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        [Required, Column("Day")]
        public override DateTime Key { get; set; }

        public byte[] EnergyCurvesEveryMeter { get; set; }

        [NotMapped]
        public TimeSeriesStreamCollection<Dsuid, int> EnergySeriesEveryMeter
        {
            get => new TimeSeriesStreamCollection<Dsuid, int>(EnergyCurvesEveryMeter, Dsuid.Size, stream => Dsuid.ReadFrom(stream), Span, DecimalPlaces);
            set { EnergyCurvesEveryMeter = value.ToCompressedByteArray(); }
        }

        public static TimeSeriesStreamCollection<Dsuid, int> InitialEnergySeriesEveryMeter(DateTime day, IEnumerable<Dsuid> dsuids)
        {
            return new TimeSeriesStreamCollection<Dsuid, int>(dsuids, Dsuid.Size, (dsuid, stream) => dsuid.WriteTo(stream),
                new TimeSeriesSpan(day, TimeSeriesSpan.Spacing.Spacing1Sec, (int)TimeSeriesSpan.Spacing.Spacing1Day));
        }
    }
}