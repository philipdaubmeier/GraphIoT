using PhilipDaubmeier.GraphIoT.Graphite.Parser;
using System;
using Xunit;

namespace GraphIoT.Graphite.Tests
{
    public class TimeExtensionTest
    {
        [Theory]
        [InlineData("today", 2015, 6, 10)]
        [InlineData("yesterday", 2015, 6, 9)]
        [InlineData("tomorrow", 2015, 6, 11)]
        public void TestDates(string s, int expectedYear, int expectedMonth, int expectedDay)
        {
            TimeExtensions.Now = new DateTime(2015, 6, 10, 0, 0, 0, DateTimeKind.Utc);

            Assert.True(s.TryParseGraphiteTime(out DateTime res));
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, 0, 0, 0, DateTimeKind.Utc), res);
        }

        [Theory]
        [InlineData("midnight", 2015, 6, 10, 0, 0, 0)]
        [InlineData("midnight+1h", 2015, 6, 10, 1, 0, 0)]
        [InlineData("midnight_tomorrow", 2015, 6, 11, 0, 0, 0)]
        [InlineData("midnight_tomorrow+3h", 2015, 6, 11, 3, 0, 0)]
        [InlineData("midnight_tomorrow+3h27min", 2015, 6, 11, 3, 27, 0)]
        [InlineData("midnight_tomorrow+3h27min9s", 2015, 6, 11, 3, 27, 9)]
        [InlineData("midnight_tomorrow+33s", 2015, 6, 11, 0, 0, 33)]
        public void TestRelativeTimes(string s, int expectedYear, int expectedMonth, int expectedDay, int expectedHours, int expectedMinutes, int expectedSeconds)
        {
            TimeExtensions.Now = new DateTime(2015, 6, 10, 0, 0, 0, DateTimeKind.Utc);

            var time = s.ParseGraphiteTime();

            Assert.True(s.TryParseGraphiteTime(out DateTime res));
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), time);
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), res);
        }
    }
}