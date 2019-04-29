using NodaTime;
using PhilipDaubmeier.CompactTimeSeries;
using System;
using Xunit;

namespace PhilipDaubmeier.TimeseriesHostCommon.Parsers.Tests
{
    public class TimeSeriesSpanParserTest
    {
        [Theory]
        [InlineData("0", 1, true)]
        [InlineData("23", 23, true)]
        [InlineData(" 23  \t", 23, true)]
        [InlineData("6754", 5000, true)]
        [InlineData("-123", 1, true)]
        [InlineData("foo", 0, false)]
        public void TestTimeSeriesSpanTryParse(string countIn, int expectedCount, bool expectedParsingResult)
        {
            var millisecPrecision = TimeSpan.FromMilliseconds(1);
            var beginTime = DateTime.UtcNow;
            var endTime = beginTime.AddDays(1);

            var begin = Instant.FromDateTimeUtc(beginTime).ToUnixTimeMilliseconds().ToString();
            var end = Instant.FromDateTimeUtc(endTime).ToUnixTimeMilliseconds().ToString();

            Assert.Equal(expectedParsingResult, TimeSeriesSpanParser.TryParse(begin, end, countIn, out TimeSeriesSpan span));

            if (!expectedParsingResult)
                return;

            Assert.Equal(beginTime, span.Begin, millisecPrecision);
            Assert.Equal(endTime, span.End, millisecPrecision);
            Assert.Equal(expectedCount, span.Count);
            Assert.Equal((endTime - beginTime) / expectedCount, span.Duration);
        }
    }
}