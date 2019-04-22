using PhilipDaubmeier.CompactTimeSeries;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public class GraphViewModel
    {
        public DateTime Begin { get; set; }
        public long BeginUnixTimestamp => Instant.FromDateTimeUtc(Begin).ToUnixTimeMilliseconds();

        public TimeSpan Spacing { get; set; }
        public long SpacingMillis => (long)Spacing.TotalMilliseconds;

        public string Name { get; set; }
        public string Format { get; set; }
        public IEnumerable<dynamic> Points { get; set; }

        public GraphViewModel()
        {
            Begin = DateTime.MinValue;
            Spacing = TimeSpan.FromMilliseconds(0);
            Name = string.Empty;
            Format = string.Empty;
            Points = new List<dynamic>();
        }

        public IEnumerable<dynamic[]> TimestampedPoints()
        {
            var timestamp = BeginUnixTimestamp;
            foreach (var point in Points)
            {
                yield return new dynamic[] { point, timestamp };
                timestamp += SpacingMillis;
            }
        }
    }

    public class GraphViewModel<T> : GraphViewModel where T : struct
    {
        public GraphViewModel(ITimeSeries<T> timeseries, string name = null, string format = null)
        {
            Name = name ?? string.Empty;
            Format = format ?? string.Empty;
            Begin = FindBegin(timeseries);
            Spacing = timeseries.Span.Duration;
            Points = timeseries.Trimmed().Cast<dynamic>().ToList();
        }

        private DateTime FindBegin(ITimeSeries<T> timeseries)
        {
            return timeseries.SkipWhile(t => !t.Value.HasValue).FirstOrDefault().Key.ToUniversalTime();
        }
    }
}