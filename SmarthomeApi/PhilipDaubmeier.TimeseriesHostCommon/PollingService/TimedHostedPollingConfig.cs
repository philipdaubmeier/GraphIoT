using System;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.TimeseriesHostCommon
{
    public class TimedHostedPollingConfig<TPollingService> where TPollingService : IScopedPollingService
    {
        public string Name { get; set; }
        public string TimerInterval { get; set; }

        public TimeSpan TimerIntervalTimeSpan
        {
            get
            {
                var match = Regex.Match(TimerInterval, @"^((?<h>\d+)h)?((?<m>\d+)m)?((?<s>\d+)s)?$",
                    RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.RightToLeft);

                var h = match.Groups["h"].Success ? int.Parse(match.Groups["h"].Value) : 0;
                var m = match.Groups["m"].Success ? int.Parse(match.Groups["m"].Value) : 0;
                var s = match.Groups["s"].Success ? int.Parse(match.Groups["s"].Value) : 0;

                return new TimeSpan(h, m, s);
            }
        }
    }
}