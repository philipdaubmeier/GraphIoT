using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;

namespace PhilipDaubmeier.TimeseriesHostCommon
{
    public class TimedHostedPollingConfig<TPollingService> where TPollingService : IScopedPollingService
    {
        public string Name { get; set; }
        public string TimerInterval { get; set; }

        public TimeSpan TimerIntervalTimeSpan => TimerInterval.ToTimeSpan();
    }
}