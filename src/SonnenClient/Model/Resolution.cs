using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.SonnenClient.Model
{
    /// <summary>
    /// Immutable utility primitive wrapper to parse a duration in the form of
    /// e.g. "1m", "5h10m", etc. to use it as a TimeSpan via implicit casting.
    /// </summary>
    [DebuggerDisplay("Source = {_original} ; Parsed = {_resolution.ToString(),nq}")]
    public class Resolution
    {
        private readonly string _original;
        private readonly TimeSpan _resolution;

        public Resolution(string resolution)
        {
            _original = resolution;

            var match = Regex.Match(resolution, @"^((?<h>\d+)(h|-hour))?((?<m>\d+)m)?((?<s>\d+)s)?((?<ms>\d+)ms)?$",
                RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.RightToLeft);

            var h = match.Groups["h"].Success ? int.Parse(match.Groups["h"].Value) : 0;
            var m = match.Groups["m"].Success ? int.Parse(match.Groups["m"].Value) : 0;
            var s = match.Groups["s"].Success ? int.Parse(match.Groups["s"].Value) : 0;
            var ms = match.Groups["ms"].Success ? int.Parse(match.Groups["ms"].Value) : 0;

            _resolution = new TimeSpan(0, h, m, s, ms);
        }

        public override string ToString()
        {
            return _original.ToString();
        }

        public static implicit operator Resolution(string resolution)
        {
            return new Resolution(resolution);
        }

        public static implicit operator TimeSpan(Resolution resolution)
        {
            return resolution._resolution;
        }
    }
}