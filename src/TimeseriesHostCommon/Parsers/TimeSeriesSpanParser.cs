using PhilipDaubmeier.CompactTimeSeries;
using NodaTime;
using System;

namespace PhilipDaubmeier.TimeseriesHostCommon.Parsers
{
    public static class TimeSeriesSpanParser
    {
        private const int minCount = 1;
        private const int maxCount = 5000;

        public static bool TryParse(string begin, string end, string count, out TimeSeriesSpan span)
        {
            span = null;

            if (!int.TryParse(count, out int countInt))
                return false;

            var countParsed = Math.Min(Math.Max(countInt, minCount), maxCount);

            if (long.TryParse(begin, out long beginMillis) && long.TryParse(end, out long endMillis))
            {
                span = new TimeSeriesSpan(Instant.FromUnixTimeMilliseconds(beginMillis).ToDateTimeUtc(),
                    Instant.FromUnixTimeMilliseconds(endMillis).ToDateTimeUtc(), countParsed);
                return true;
            }

            if (DateTime.TryParse(begin, out DateTime beginDate) && DateTime.TryParse(end, out DateTime endDate))
            {
                span = new TimeSeriesSpan(beginDate.ToUniversalTime(), endDate.ToUniversalTime(), countParsed);
                return true;
            }

            return false;
        }
    }
}