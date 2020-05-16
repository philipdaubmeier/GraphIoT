using PhilipDaubmeier.GraphIoT.Graphite.Parser;
using System;
using Xunit;

namespace GraphIoT.Graphite.Tests
{
    public class TimeExtensionTest
    {
        [Theory]
        [InlineData("today", 2015, 6, 10, 0, 0, 0)]
        [InlineData("yesterday", 2015, 6, 9, 0, 0, 0)]
        [InlineData("tomorrow", 2015, 6, 11, 0, 0, 0)]
        [InlineData("now", 2015, 6, 10, 0, 0, 0)]
        [InlineData("now+1h", 2015, 6, 10, 1, 0, 0)]
        [InlineData("now-1h", 2015, 6, 9, 23, 0, 0)]
        [InlineData("+1h", 2015, 6, 10, 1, 0, 0)]
        [InlineData("-1h", 2015, 6, 9, 23, 0, 0)]
        [InlineData("+1s", 2015, 6, 10, 0, 0, 1)]
        [InlineData("+1min", 2015, 6, 10, 0, 1, 0)]
        [InlineData("+1d", 2015, 6, 11, 0, 0, 0)]
        [InlineData("+1w", 2015, 6, 17, 0, 0, 0)]
        [InlineData("+1mon", 2015, 7, 10, 0, 0, 0)]
        [InlineData("+1y", 2016, 6, 9, 0, 0, 0)]
        [InlineData("+5d1min27s", 2015, 6, 15, 0, 1, 27)]
        [InlineData("+10days", 2015, 6, 20, 0, 0, 0)]
        [InlineData("+0days", 2015, 6, 10, 0, 0, 0)]
        [InlineData("-10days", 2015, 5, 31, 0, 0, 0)]
        [InlineData("+5seconds", 2015, 6, 10, 0, 0, 5)]
        [InlineData("+5minutes", 2015, 6, 10, 0, 5, 0)]
        [InlineData("+5hours", 2015, 6, 10, 5, 0, 0)]
        [InlineData("+5weeks", 2015, 7, 15, 0, 0, 0)]
        [InlineData("+1month", 2015, 7, 10, 0, 0, 0)]
        [InlineData("+2months", 2015, 8, 9, 0, 0, 0)] // this is not the same day 2 months later, however, this is following the original attime.py implementation
        [InlineData("+12months", 2016, 6, 4, 0, 0, 0)] // this is not the same day 12 months later, however, this is following the original attime.py implementation
        [InlineData("+1year", 2016, 6, 9, 0, 0, 0)] // this is not the same day 1 year later, however, this is following the original attime.py implementation
        [InlineData("+2years", 2017, 6, 9, 0, 0, 0)] // this is not the same day 2 years later, however, this is following the original attime.py implementation
        [InlineData("0:00", 2015, 6, 10, 0, 0, 0)]
        [InlineData("00:00", 2015, 6, 10, 0, 0, 0)]
        [InlineData("12:00", 2015, 6, 10, 12, 0, 0)]
        [InlineData("23:59", 2015, 6, 10, 23, 59, 0)]
        [InlineData("24:00", 2015, 6, 11, 0, 0, 0)]
        [InlineData("23:59+1h", 2015, 6, 11, 0, 59, 0)]
        [InlineData("11:23_today", 2015, 6, 10, 11, 23, 0)]
        [InlineData("11:23_today+1h", 2015, 6, 10, 12, 23, 0)]
        [InlineData("11:23_tomorrow", 2015, 6, 11, 11, 23, 0)]
        [InlineData("11:23_yesterday", 2015, 6, 9, 11, 23, 0)]
        [InlineData("0:00am", 2015, 6, 10, 0, 0, 0)]
        [InlineData("00:00am", 2015, 6, 10, 0, 0, 0)]
        [InlineData("12:00am", 2015, 6, 10, 12, 0, 0)]
        [InlineData("1:00am", 2015, 6, 10, 1, 0, 0)]
        [InlineData("1am", 2015, 6, 10, 1, 0, 0)]
        [InlineData("11am", 2015, 6, 10, 11, 0, 0)]
        [InlineData("11am_today", 2015, 6, 10, 11, 0, 0)]
        [InlineData("11am_tomorrow", 2015, 6, 11, 11, 0, 0)]
        [InlineData("11am_yesterday", 2015, 6, 9, 11, 0, 0)]
        [InlineData("0:00pm", 2015, 6, 10, 12, 0, 0)]
        [InlineData("00:00pm", 2015, 6, 10, 12, 0, 0)]
        [InlineData("12:00pm", 2015, 6, 10, 0, 0, 0)]
        [InlineData("1:00pm", 2015, 6, 10, 13, 0, 0)]
        [InlineData("1pm", 2015, 6, 10, 13, 0, 0)]
        [InlineData("11pm", 2015, 6, 10, 23, 0, 0)]
        [InlineData("11pm_today", 2015, 6, 10, 23, 0, 0)]
        [InlineData("11pm_tomorrow", 2015, 6, 11, 23, 0, 0)]
        [InlineData("11pm_yesterday", 2015, 6, 9, 23, 0, 0)]
        [InlineData("12:0020150308", 2015, 3, 8, 12, 0, 0)]
        [InlineData("9:0020150308", 2015, 3, 8, 9, 0, 0)]
        public void TestTimeOfDay(string s, int expectedYear, int expectedMonth, int expectedDay, int expectedHours, int expectedMinutes, int expectedSeconds)
        {
            TimeExtensions.Now = new DateTime(2015, 6, 10, 0, 0, 0, DateTimeKind.Utc);

            var time = s.ParseGraphiteTime();

            Assert.True(s.TryParseGraphiteTime(out DateTime res));
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), time);
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), res);
        }

        [Theory]
        [InlineData("midnight", 2015, 6, 10, 0, 0, 0)]
        [InlineData("midnight+1h", 2015, 6, 10, 1, 0, 0)]
        [InlineData("midnight tomorrow", 2015, 6, 11, 0, 0, 0)]
        [InlineData("midnight_tomorrow", 2015, 6, 11, 0, 0, 0)]
        [InlineData("midnight_tomorrow+3h", 2015, 6, 11, 3, 0, 0)]
        [InlineData("midnight_tomorrow+3h27min", 2015, 6, 11, 3, 27, 0)]
        [InlineData("midnight_tomorrow+3h27min9s", 2015, 6, 11, 3, 27, 9)]
        [InlineData("midnight_tomorrow+33s", 2015, 6, 11, 0, 0, 33)]
        [InlineData("noon", 2015, 6, 10, 12, 0, 0)]
        [InlineData("noon+1h", 2015, 6, 10, 13, 0, 0)]
        [InlineData("noon tomorrow", 2015, 6, 11, 12, 0, 0)]
        [InlineData("noon_tomorrow", 2015, 6, 11, 12, 0, 0)]
        [InlineData("noon_tomorrow+3h", 2015, 6, 11, 15, 0, 0)]
        [InlineData("noon_tomorrow+3h27min", 2015, 6, 11, 15, 27, 0)]
        [InlineData("noon_tomorrow+3h27min9s", 2015, 6, 11, 15, 27, 9)]
        [InlineData("noon_tomorrow+33s", 2015, 6, 11, 12, 0, 33)]
        [InlineData("teatime", 2015, 6, 10, 16, 0, 0)]
        [InlineData("teatime+1h", 2015, 6, 10, 17, 0, 0)]
        [InlineData("teatime tomorrow", 2015, 6, 11, 16, 0, 0)]
        [InlineData("teatime_tomorrow", 2015, 6, 11, 16, 0, 0)]
        [InlineData("teatime_tomorrow+3h", 2015, 6, 11, 19, 0, 0)]
        [InlineData("teatime_tomorrow+3h27min", 2015, 6, 11, 19, 27, 0)]
        [InlineData("teatime_tomorrow+3h27min9s", 2015, 6, 11, 19, 27, 9)]
        [InlineData("teatime_tomorrow+33s", 2015, 6, 11, 16, 0, 33)]
        public void TestNamedTimes(string s, int expectedYear, int expectedMonth, int expectedDay, int expectedHours, int expectedMinutes, int expectedSeconds)
        {
            TimeExtensions.Now = new DateTime(2015, 6, 10, 0, 0, 0, DateTimeKind.Utc);

            var time = s.ParseGraphiteTime();

            Assert.True(s.TryParseGraphiteTime(out DateTime res));
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), time);
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), res);
        }

        [Theory]
        [InlineData("01/01/1900", 1900, 1, 1, 0, 0, 0)] // MM/DD/YYYY
        [InlineData("01/01/2000", 2000, 1, 1, 0, 0, 0)] // MM/DD/YYYY
        [InlineData("12/31/2000", 2000, 12, 31, 0, 0, 0)] // MM/DD/YYYY
        [InlineData("02/29/2000", 2000, 2, 29, 0, 0, 0)] // MM/DD/YYYY
        [InlineData("01/01/0950", 950, 1, 1, 0, 0, 0)] // MM/DD/YYYY
        [InlineData("01/01/3000", 3000, 1, 1, 0, 0, 0)] // MM/DD/YYYY
        [InlineData("01/01/15", 2015, 1, 1, 0, 0, 0)] // MM/DD/YY
        [InlineData("01/01/69", 2069, 1, 1, 0, 0, 0)] // MM/DD/YY
        [InlineData("01/01/70", 1970, 1, 1, 0, 0, 0)] // MM/DD/YY
        [InlineData("01/01/71", 1971, 1, 1, 0, 0, 0)] // MM/DD/YY
        [InlineData("01/01/99", 1999, 1, 1, 0, 0, 0)] // MM/DD/YY
        [InlineData("19000101", 1900, 1, 1, 0, 0, 0)] // YYYYMMDD
        [InlineData("20000101", 2000, 1, 1, 0, 0, 0)] // YYYYMMDD
        [InlineData("20001231", 2000, 12, 31, 0, 0, 0)] // YYYYMMDD
        [InlineData("20000229", 2000, 2, 29, 0, 0, 0)] // YYYYMMDD
        [InlineData("30000101", 3000, 1, 1, 0, 0, 0)] // YYYYMMDD
        [InlineData("0", 1970, 1, 1, 0, 0, 0)] // unix timestamp
        [InlineData("18999999", 1970, 8, 8, 21, 46, 39)] // unix timestamp (largest that is not recognized as YYYYMMDD)
        [InlineData("99991300", 1973, 3, 3, 7, 21, 40)] // unix timestamp (smalles from which on no more number is recognized as YYYYMMDD)
        [InlineData("1433894400", 2015, 6, 10, 0, 0, 0)] // unix timestamp
        [InlineData("jan1", 2015, 1, 1, 0, 0, 0)] // MonthName DayOfMonth
        [InlineData("jan01", 2015, 1, 1, 0, 0, 0)] // MonthName DayOfMonth
        [InlineData("january01", 2015, 1, 1, 0, 0, 0)] // MonthName DayOfMonth
        [InlineData("january_01", 2015, 1, 1, 0, 0, 0)] // MonthName DayOfMonth
        [InlineData("january 01", 2015, 1, 1, 0, 0, 0)] // MonthName DayOfMonth
        [InlineData("febuary28", 2015, 2, 28, 0, 0, 0)] // MonthName DayOfMonth
        [InlineData("december6", 2015, 12, 6, 0, 0, 0)] // MonthName DayOfMonth
        [InlineData("dec_some_gibberish_6", 2015, 12, 6, 0, 0, 0)] // MonthName only has to match first 3 letters
        [InlineData("thursday", 2015, 6, 4, 0, 0, 0)] // DayOfWeek
        [InlineData("sunday", 2015, 6, 7, 0, 0, 0)] // DayOfWeek
        [InlineData("monday", 2015, 6, 8, 0, 0, 0)] // DayOfWeek
        [InlineData("wed", 2015, 6, 10, 0, 0, 0)] // DayOfWeek
        public void TestDateFormats(string s, int expectedYear, int expectedMonth, int expectedDay, int expectedHours, int expectedMinutes, int expectedSeconds)
        {
            TimeExtensions.Now = new DateTime(2015, 6, 10, 0, 0, 0, DateTimeKind.Utc);

            var time = s.ParseGraphiteTime();

            Assert.True(s.TryParseGraphiteTime(out DateTime res));
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), time);
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), res);
        }

        [Theory]
        [InlineData("yesterday", 2015, 6, 9, 0, 0, 0)]
        [InlineData("Yesterday", 2015, 6, 9, 0, 0, 0)]
        [InlineData("YesTERDay", 2015, 6, 9, 0, 0, 0)]
        [InlineData("YESTERDAY", 2015, 6, 9, 0, 0, 0)]
        [InlineData("yes_ter_day", 2015, 6, 9, 0, 0, 0)]
        [InlineData("Yes_TER_Day", 2015, 6, 9, 0, 0, 0)]
        [InlineData("Yes_T__ _E  R_Day", 2015, 6, 9, 0, 0, 0)]
        public void TestWhitespaceAndCaseResilience(string s, int expectedYear, int expectedMonth, int expectedDay, int expectedHours, int expectedMinutes, int expectedSeconds)
        {
            TimeExtensions.Now = new DateTime(2015, 6, 10, 0, 0, 0, DateTimeKind.Utc);

            var time = s.ParseGraphiteTime();

            Assert.True(s.TryParseGraphiteTime(out DateTime res));
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), time);
            Assert.Equal(new DateTime(expectedYear, expectedMonth, expectedDay, expectedHours, expectedMinutes, expectedSeconds, DateTimeKind.Utc), res);
        }

        /// <summary>
        /// All tests data tuples with expectedToThrow == false should not throw because
        /// they are in principle invalid but do not throw in original attime.py implementation
        /// </summary>
        [Theory]
        [InlineData("0:0", true)]
        [InlineData("5xm", true)]
        [InlineData("20am", false)]
        [InlineData("100am", true)]
        [InlineData("25:00", false)]
        [InlineData("25:60", false)]
        [InlineData("january", true)]
        [InlineData("january800", true)]
        [InlineData(":", true)]
        [InlineData("Something", true)]
        [InlineData("1m", true)]
        [InlineData("noon+yesterday", false)]
        [InlineData("random", true)]
        public void TestInvalidValues(string s, bool expectedToThrow)
        {
            TimeExtensions.Now = new DateTime(2015, 6, 10, 0, 0, 0, DateTimeKind.Utc);

            if (expectedToThrow)
            {
                Assert.False(s.TryParseGraphiteTime(out DateTime _));
                Assert.ThrowsAny<Exception>(() => s.ParseGraphiteTime());
            }
            else
            {
                Assert.True(s.TryParseGraphiteTime(out DateTime _));
                s.ParseGraphiteTime();
            }
        }
    }
}