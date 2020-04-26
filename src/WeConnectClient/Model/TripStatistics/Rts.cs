using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    public class Rts
    {
        public int DaysInMonth { get; set; }
        public int FirstWeekday { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int FirstTripYear { get; set; }

        public List<TripStatisticGroup> TripStatistics { get; set; } = new List<TripStatisticGroup>();

        public TripStatisticEntry LongTermData { get; set; } = new TripStatisticEntry();

        public object CyclicData { get; set; } = string.Empty;
        public RtsConfiguration ServiceConfiguration { get; set; } = new RtsConfiguration();
        public bool TripFromLastRefuelAvailable { get; set; }
    }
}