using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    public class Rts
    {
        [JsonIgnore]
        internal CultureInfo DateTimeLocale { get; set; } = CultureInfo.InvariantCulture;

        public int DaysInMonth { get; set; }
        public int FirstWeekday { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int FirstTripYear { get; set; }

        public List<TripStatisticGroup> TripStatistics { get; set; } = new List<TripStatisticGroup>();

        public TripStatisticEntry LongTermData { get; set; } = new TripStatisticEntry();

        public TripStatisticEntry CyclicData { get; set; } = new TripStatisticEntry();
        public RtsConfiguration ServiceConfiguration { get; set; } = new RtsConfiguration();
        public bool TripFromLastRefuelAvailable { get; set; }
    }
}