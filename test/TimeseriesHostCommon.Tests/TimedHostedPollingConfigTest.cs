using System;
using System.Collections.Generic;
using Xunit;

namespace PhilipDaubmeier.TimeseriesHostCommon.Tests
{
    public class TimedHostedPollingConfigTest
    {
        [Theory]
        [MemberData(nameof(GetTimeSpans))]
        public void TestTimerIntervalTimeSpan(string input, TimeSpan output)
        {
            var config = new TimedHostedPollingConfig<IScopedPollingService>();
            config.TimerInterval = input;

            Assert.Equal(output, config.TimerIntervalTimeSpan);
        }

        public static IEnumerable<object[]> GetTimeSpans()
        {
            return new List<object[]>
            {
                new object[] { "1h", TimeSpan.FromHours(1) },
                new object[] { "1m", TimeSpan.FromMinutes(1) },
                new object[] { "1s", TimeSpan.FromSeconds(1) },
                new object[] { " 1s ", TimeSpan.FromSeconds(0) },
                new object[] { "foo", TimeSpan.FromSeconds(0) },
                new object[] { "23s", TimeSpan.FromSeconds(23) },
                new object[] { "60s", TimeSpan.FromMinutes(1) },
                new object[] { "1d", TimeSpan.FromSeconds(0) }
            };
        }
    }
}