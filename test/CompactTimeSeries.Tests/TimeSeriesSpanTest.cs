using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.CompactTimeSeries.Tests
{
    public class TimeSeriesSpanTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);
        
        [Fact]
        public void TestTimeSeriesSpanConstructors()
        {
            var span1 = new TimeSeriesSpan(begin, end, count);
            var span2 = new TimeSeriesSpan(begin, end, TimeSeriesSpan.Spacing.Spacing1Min);
            var span3 = new TimeSeriesSpan(begin, TimeSeriesSpan.Spacing.Spacing1Min, count);
            var span4 = new TimeSeriesSpan(end, begin, count);

            Assert.Equal(span1.Begin, span2.Begin);
            Assert.Equal(span1.End, span2.End);
            Assert.Equal(span1.Duration, span2.Duration);
            Assert.Equal(span1.Count, span2.Count);

            Assert.Equal(span1.Begin, span3.Begin);
            Assert.Equal(span1.End, span3.End);
            Assert.Equal(span1.Duration, span3.Duration);
            Assert.Equal(span1.Count, span3.Count);

            Assert.Equal(span1.Begin, span4.Begin);
            Assert.Equal(span1.End, span4.End);
            Assert.Equal(span1.Duration, span4.Duration);
            Assert.Equal(span1.Count, span4.Count);
        }

        [Fact]
        public void TestTimeSeriesSpanIncludedDates()
        {
            var d1 = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Local);
            var d2 = new DateTime(2019, 01, 10, 13, 23, 00, DateTimeKind.Local);
            var d3 = new DateTime(2019, 01, 11, 13, 23, 00, DateTimeKind.Local);
            var d4 = new DateTime(2019, 01, 12, 13, 23, 00, DateTimeKind.Local);
            var d1a = new DateTime(2019, 01, 09, 00, 00, 00, DateTimeKind.Local);
            var d2a = new DateTime(2019, 01, 10, 00, 00, 00, DateTimeKind.Local);

            Assert.Equal(new List<DateTime>() { d1.Date }, new TimeSeriesSpan(d1, d1, count).IncludedDates().ToList());
            Assert.Equal(new List<DateTime>() { d1.Date, d2.Date }, new TimeSeriesSpan(d1, d2, count).IncludedDates().ToList());
            Assert.Equal(new List<DateTime>() { d1.Date, d2.Date }, new TimeSeriesSpan(d2, d1, count).IncludedDates().ToList());
            Assert.Equal(new List<DateTime>() { d1.Date, d2.Date, d3.Date }, new TimeSeriesSpan(d1, d3, count).IncludedDates().ToList());
            Assert.Equal(new List<DateTime>() { d1.Date, d2.Date, d3.Date }, new TimeSeriesSpan(d3, d1, count).IncludedDates().ToList());
            Assert.Equal(new List<DateTime>() { d1.Date, d2.Date, d3.Date, d4.Date }, new TimeSeriesSpan(d1, d4, count).IncludedDates().ToList());
            Assert.Equal(new List<DateTime>() { d1a.Date, d2a.Date }, new TimeSeriesSpan(d1a, d2a, count).IncludedDates().ToList());
        }
    }
}