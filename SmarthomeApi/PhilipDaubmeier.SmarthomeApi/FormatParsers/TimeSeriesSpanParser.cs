using PhilipDaubmeier.CompactTimeSeries;
using NodaTime;
using System;

namespace PhilipDaubmeier.SmarthomeApi.FormatParsers
{
    public static class TimeSeriesSpanParser
    {
        private const int minCount = 1;
        private const int maxCount = 5000;

        public static bool TryParse(string begin, string end, string count, out TimeSeriesSpan span)
        {
            span = null;

            if (!long.TryParse(begin, out long beginMillis) || !long.TryParse(end, out long endMillis) || !int.TryParse(count, out int countInt))
                return false;

            span = new TimeSeriesSpan(Instant.FromUnixTimeMilliseconds(beginMillis).ToDateTimeUtc(),
                Instant.FromUnixTimeMilliseconds(endMillis).ToDateTimeUtc(), Math.Min(Math.Max(countInt, minCount), maxCount));

            return true;
        }
    }
}