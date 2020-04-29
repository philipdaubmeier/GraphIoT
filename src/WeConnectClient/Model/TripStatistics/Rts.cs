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

        public object CyclicData { get; set; } = string.Empty;
        public RtsConfiguration ServiceConfiguration { get; set; } = new RtsConfiguration();
        public bool TripFromLastRefuelAvailable { get; set; }

        [JsonIgnore]
        public IEnumerable<(DateTime start, TimeSpan duration, TripStatisticEntry trip)> AllEntries
        {
            get
            {
                DateTime ParseTimeStamp(string formattedTimestamp, int fallbackDay)
                {
                    var timeString = formattedTimestamp.Split(':', 2).Last().Trim();

                    if (DateTime.TryParse(timeString, DateTimeLocale, DateTimeStyles.AssumeLocal, out DateTime timestamp))
                        return timestamp;

                    return new DateTime(Year, Month, fallbackDay);
                }

                return Enumerable.Range(1, DaysInMonth)
                    .Zip(TripStatistics, (day, trips) => (day, trips))
                    .SelectMany(x => x.trips?.TripStatistics?.Select(trip => (x.day, trip)).ToList() ?? new List<(int, TripStatisticEntry)>())
                    .Select(x => (end: ParseTimeStamp(x.trip.LongFormattedTimestamp, x.day), duration: TimeSpan.FromMinutes(x.trip.TripDuration), x.trip))
                    .Select(x => (start: x.end.Subtract(x.duration), x.duration, x.trip));
            }
        }
    }
}