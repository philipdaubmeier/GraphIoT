using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    public class TripStatisticGroup
    {
        public TripStatisticEntry AggregatedStatistics { get; set; } = new TripStatisticEntry();
        public List<TripStatisticEntry> TripStatistics { get; set; } = new List<TripStatisticEntry>();
    }
}