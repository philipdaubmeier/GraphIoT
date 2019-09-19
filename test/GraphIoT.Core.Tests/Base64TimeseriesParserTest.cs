using PhilipDaubmeier.CompactTimeSeries;
using System;
using Xunit;

namespace PhilipDaubmeier.GraphIoT.Core.Parsers.Tests
{
    public class TimeSeriesTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);
        private static readonly TimeSeriesSpan span = new TimeSeriesSpan(begin, end, count);

        private static readonly DateTime insideRangeExactSecond = new DateTime(2019, 01, 09, 13, 28, 00, DateTimeKind.Utc);
        private static readonly DateTime insideRangeOddSecond1 = new DateTime(2019, 01, 09, 13, 26, 01, DateTimeKind.Utc);
        private static readonly DateTime insideRangeOddSecond2 = new DateTime(2019, 01, 09, 13, 30, 59, DateTimeKind.Utc);

        [Fact]
        public void TestIntTimeSeriesWriter()
        {
            var timeseries = new TimeSeries<int>(span);

            Assert.Equal("gACAAIAAgACAAIAAgACAAIAAgAA=", timeseries.ToBase64());

            timeseries[insideRangeExactSecond] = 23;
            Assert.Equal("gACAAIAAgACAAAAXgACAAIAAgAA=", timeseries.ToBase64());

            timeseries[insideRangeOddSecond1] = 42;
            Assert.Equal("gACAAIAAACqAAAAXgACAAIAAgAA=", timeseries.ToBase64());

            timeseries[insideRangeOddSecond2] = 43;
            Assert.Equal("gACAAIAAACqAAAAXgAAAK4AAgAA=", timeseries.ToBase64());

            timeseries[begin] = 1;
            Assert.Equal("AAGAAIAAACqAAAAXgAAAK4AAgAA=", timeseries.ToBase64());

            timeseries[end] = 99;
            Assert.Equal("AAGAAIAAACqAAAAXgAAAK4AAAGM=", timeseries.ToBase64());
        }

        [Fact]
        public void TestDoubleTimeSeriesWriter()
        {
            var timeseries = new TimeSeries<double>(span);

            Assert.Equal("gACAAIAAgACAAIAAgACAAIAAgAA=", timeseries.ToBase64());

            timeseries[insideRangeExactSecond] = 2.3;
            Assert.Equal("gACAAIAAgACAAAAXgACAAIAAgAA=", timeseries.ToBase64());

            timeseries[insideRangeOddSecond1] = 4.2;
            Assert.Equal("gACAAIAAACqAAAAXgACAAIAAgAA=", timeseries.ToBase64());

            timeseries[insideRangeOddSecond2] = 4.3;
            Assert.Equal("gACAAIAAACqAAAAXgAAAK4AAgAA=", timeseries.ToBase64());

            timeseries[begin] = .1;
            Assert.Equal("AAGAAIAAACqAAAAXgAAAK4AAgAA=", timeseries.ToBase64());

            timeseries[end] = 9.9;
            Assert.Equal("AAGAAIAAACqAAAAXgAAAK4AAAGM=", timeseries.ToBase64());
        }

        [Fact]
        public void TestBoolTimeSeriesWriter()
        {
            var timeseries = new TimeSeries<bool>(span);

            Assert.Equal("////", timeseries.ToBase64());

            timeseries[insideRangeExactSecond] = false;
            Assert.Equal("/8//", timeseries.ToBase64());

            timeseries[insideRangeOddSecond1] = true;
            Assert.Equal("/c//", timeseries.ToBase64());
        }

        [Fact]
        public void TestIntTimeSeriesParser()
        {
            var timeseries = "AAGAAIAAACqAAAAXgAAAK4AAAGM=".ToTimeseries<int>(span);

            Assert.Equal(23, timeseries[insideRangeExactSecond]);
            Assert.Equal(42, timeseries[insideRangeOddSecond1]);
            Assert.Equal(43, timeseries[insideRangeOddSecond2]);
            Assert.Equal(1, timeseries[begin]);
            Assert.Equal(99, timeseries[end]);
        }

        [Fact]
        public void TestDoubleTimeSeriesParser()
        {
            var timeseries = "AAGAAIAAACqAAAAXgAAAK4AAAGM=".ToTimeseries<double>(span);

            Assert.Equal(2.3, timeseries[insideRangeExactSecond]);
            Assert.Equal(4.2, timeseries[insideRangeOddSecond1]);
            Assert.Equal(4.3, timeseries[insideRangeOddSecond2]);
            Assert.Equal(.1, timeseries[begin]);
            Assert.Equal(9.9, timeseries[end]);

            var timeseries2 = "AAGAAIAAACqAAAAXgAAAK4AAAGM=".ToTimeseries<double>(span, 0);

            Assert.Equal(23, timeseries2[insideRangeExactSecond]);
            Assert.Equal(42, timeseries2[insideRangeOddSecond1]);
            Assert.Equal(43, timeseries2[insideRangeOddSecond2]);
            Assert.Equal(1, timeseries2[begin]);
            Assert.Equal(99, timeseries2[end]);
        }

        [Fact]
        public void TestBoolTimeSeriesParser()
        {
            var timeseries = "/c//".ToTimeseries<bool>(span);

            Assert.False(timeseries[insideRangeExactSecond]);
            Assert.True(timeseries[insideRangeOddSecond1]);
            Assert.Null(timeseries[insideRangeOddSecond2]);
        }
    }
}