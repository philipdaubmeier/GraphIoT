using PhilipDaubmeier.CompactTimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    public class GraphViewModel
    {
        public DateTime Begin { get; set; }
        public long BeginUnixTimestamp => new DateTimeOffset(Begin.ToUniversalTime()).ToUnixTimeMilliseconds();

        public TimeSpan Spacing { get; set; }
        public long SpacingMillis => (long)Spacing.TotalMilliseconds;

        public string Name { get; set; }
        public string Key { get; set; }
        public string Format { get; set; }
        public IEnumerable<dynamic> Points { get; set; }

        public GraphViewModel()
        {
            Begin = DateTimeOffset.FromUnixTimeMilliseconds(0).UtcDateTime;
            Spacing = TimeSpan.FromMilliseconds(0);
            Name = string.Empty;
            Key = string.Empty;
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
        public GraphViewModel(ITimeSeries<T> timeseries, string name, string format)
            : this(timeseries, name, name, format) { }

        public GraphViewModel(ITimeSeries<T> timeseries, string name, string key, string format)
        {
            Name = name ?? string.Empty;
            Key = key ?? string.Empty;
            Format = format ?? string.Empty;
            Begin = Max(FindBegin(timeseries), DateTimeOffset.FromUnixTimeMilliseconds(0).UtcDateTime);
            Spacing = timeseries.Span.Duration;
            Points = timeseries.Trimmed().Cast<dynamic>().ToList();
        }

        private DateTime FindBegin(ITimeSeries<T> timeseries)
        {
            return timeseries.SkipWhile(t => !t.Value.HasValue).FirstOrDefault().Key.ToUniversalTime();
        }

        private DateTime Max(DateTime val1, DateTime val2)
        {
            return DateTime.FromBinary(Math.Max(val1.ToBinary(), val2.ToBinary()));
        }
    }
}