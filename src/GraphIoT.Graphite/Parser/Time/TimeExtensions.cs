using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser
{
    /// <summary>
    /// Extension methods for parsing Graphite time strings.
    ///
    /// This is a manual port to c# of Graphite's attime.py that can be found here:
    /// https://github.com/graphite-project/graphite-web/blob/master/webapp/graphite/render/attime.py
    /// </summary>
    public static class TimeExtensions
    {
        private static readonly List<string> Months = new() { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };

        private static readonly List<string> Weekdays = new() { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };

        private enum Unit
        {
            Seconds,
            Minutes,
            Hours,
            Days,
            Weeks,
            Months,
            Years
        }

        private static DateTime? _now = null;
        public static DateTime Now { get => _now ?? DateTime.UtcNow; set => _now = value; }

        public static DateTime ParseGraphiteTime(this string s)
        {
            var (result, reference, offset) = SplitReferenceOffset(s);
            return result ?? ParseTimeReference(reference!) + ParseTimeOffset(offset!);
        }

        public static bool TryParseGraphiteTime(this string s, out DateTime result)
        {
            var split = SplitReferenceOffset(s);
            if (split.result.HasValue)
            {
                result = split.result.Value;
                return true;
            }

            result = DateTime.MinValue;
            if (split.reference is null || split.offset is null)
                return false;

            try
            {
                result = ParseTimeReference(split.reference) + ParseTimeOffset(split.offset);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryParseGraphiteOffset(this string s, out TimeSpan result)
        {
            try
            {
                result = ParseTimeOffset(s);
                return true;
            }
            catch { result = new TimeSpan(); return false; }
        }

        private static (DateTime? result, string? reference, string? offset) SplitReferenceOffset(string s)
        {
            s = s.Trim().ToLowerInvariant().Replace("_", "").Replace(",", "").Replace(" ", "");

            // Only treat as timestamp if its not in YYYYMMDD form
            if (int.TryParse(s, out int number) && !(s.Length == 8
                && int.Parse(s[..4]) >= 1900 && int.Parse(s[4..6]) <= 12 && int.Parse(s[6..]) <= 31))
                return (DateTimeOffset.FromUnixTimeSeconds(number).UtcDateTime, null, null);

            var offset = string.Empty;
            void SplitOffset(char splitChar)
            {
                var split = s.Split(splitChar, 2);
                s = split.FirstOrDefault();
                offset = splitChar + split.LastOrDefault();
            }

            if (s.Contains('+'))
                SplitOffset('+');
            else if (s.Contains('-'))
                SplitOffset('-');

            return (null, s, offset);
        }

        private static DateTime ParseTimeReference(string s)
        {
            var rawRef = s;

            if (string.IsNullOrWhiteSpace(s) || s.Equals("now", StringComparison.InvariantCultureIgnoreCase))
                return Now;

            // Time-of-day reference
            var i = s.IndexOf(':');
            var hour = 0;
            var minute = 0;
            if (0 < i && i < 3 && s.Length >= i + 3
                && int.TryParse(s[..i], out hour)
                && int.TryParse(s[(i + 1)..(i + 3)], out minute))
            {
                s = s[(i + 3)..];
                if (s.Length >= 2 && s[..2].Equals("am", StringComparison.InvariantCultureIgnoreCase))
                    s = s[2..];
                else if (s.Length >= 2 && s[..2].Equals("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    hour = (hour + 12) % 24;
                    s = s[2..];
                }
            }

            // Xam or XXam
            i = s.IndexOf("am", StringComparison.InvariantCultureIgnoreCase);
            if (0 < i && i < 3 && int.TryParse(s[..i], out hour))
                s = s[(i + 2)..];

            // Xpm or XXpm
            i = s.IndexOf("pm", StringComparison.InvariantCultureIgnoreCase);
            if (0 < i && i < 3 && int.TryParse(s[..i], out hour))
            {
                hour = (hour + 12) % 24;
                s = s[(i + 2)..];
            }

            if (s.StartsWith("noon", StringComparison.InvariantCultureIgnoreCase))
                (hour, minute, s) = (12, 0, s[4..]);
            else if (s.StartsWith("midnight", StringComparison.InvariantCultureIgnoreCase))
                (hour, minute, s) = (0, 0, s[8..]);
            else if (s.StartsWith("teatime", StringComparison.InvariantCultureIgnoreCase))
                (hour, minute, s) = (16, 0, s[7..]);

            var refDate = Now.Date.AddHours(hour).AddMinutes(minute);

            // Day reference
            if (new[] { "yesterday", "today", "tomorrow" }.Contains(s.ToLowerInvariant()))
            {
                if (s.Equals("yesterday", StringComparison.InvariantCultureIgnoreCase))
                    refDate = refDate.AddDays(-1);
                else if (s.Equals("tomorrow", StringComparison.InvariantCultureIgnoreCase))
                    refDate = refDate.AddDays(1);
            }
            else if (s.Where(c => c == '/').Count() == 2)
            {
                // MM/DD/YY[YY]
                var parts = s.Split('/').Select(x => (value: int.TryParse(x, out int res) ? res : 0, length: x.Length)).ToArray();
                int m = parts[0].value, d = parts[1].value, y = parts[2].value;
                if (parts[2].length < 4)
                {
                    y += 1900;
                    if (y < 1970)
                        y += 100;
                }

                refDate = new DateTime(y, m, d, hour, minute, 0, refDate.Kind);
            }
            else if (s.Length == 8 && int.TryParse(s, out int _))
            {
                // YYYYMMDD
                refDate = new DateTime(int.Parse(s[..4]), int.Parse(s[4..6]), int.Parse(s[6..]), hour, minute, 0, refDate.Kind);
            }
            else if (s.Length >= 3 && Months.Contains(s[..3].ToLowerInvariant()))
            {
                // MonthName DayOfMonth
                if (!int.TryParse(s[^2..], out int day) && !int.TryParse(s[^1..], out day))
                    throw new ArgumentException("Day of month required after month name");

                refDate = new DateTime(refDate.Year, Months.IndexOf(s[..3].ToLowerInvariant()) + 1, day, hour, minute, 0, refDate.Kind);
            }
            else if (s.Length >= 3 && Weekdays.Contains(s[..3].ToLowerInvariant()))
            {
                // DayOfWeek (Monday, etc)
                var dayOffset = (int)refDate.DayOfWeek - Weekdays.IndexOf(s[..3].ToLowerInvariant());
                refDate = refDate.AddDays(-1 * (dayOffset < 0 ? dayOffset + 7 : dayOffset));
            }
            else if (!string.IsNullOrWhiteSpace(s))
                throw new ArgumentException($"Unknown day reference: {rawRef}");

            return refDate;
        }

        private static TimeSpan ParseTimeOffset(string offset)
        {
            if (string.IsNullOrEmpty(offset))
                return new TimeSpan();

            int sign = 1;
            if (int.TryParse(offset[0].ToString(), out int _))
                sign = 1;
            else if (offset.StartsWith('+'))
                (sign, offset) = (1, offset[1..]);
            else if (offset.StartsWith('-'))
                (sign, offset) = (-1, offset[1..]);
            else
                throw new ArgumentException($"Invalid offset: '{offset}'");

            var span = new TimeSpan();
            while (!string.IsNullOrWhiteSpace(offset))
            {
                var i = 1;
                while (i <= offset.Length && long.TryParse(offset[..i], out long _))
                    i += 1;
                if (i <= 1)
                    break;

                var num = long.Parse(offset[..(i - 1)]);
                offset = offset[(i - 1)..];
                i = 1;
                while (i <= offset.Length && offset[..i].All(c => char.IsLetter(c)))
                    i += 1;
                if (i <= 1)
                    break;

                var unit = offset[..(i - 1)];
                offset = offset[(i - 1)..];
                span += GetUnit(unit) switch
                {
                    Unit.Seconds => new TimeSpan(sign * num * TimeSpan.TicksPerSecond),
                    Unit.Minutes => new TimeSpan(sign * num * TimeSpan.TicksPerMinute),
                    Unit.Hours => new TimeSpan(sign * num * TimeSpan.TicksPerHour),
                    Unit.Days => new TimeSpan(sign * num * TimeSpan.TicksPerDay),
                    Unit.Weeks => new TimeSpan(sign * num * TimeSpan.TicksPerDay * 7),
                    Unit.Months => new TimeSpan(sign * num * TimeSpan.TicksPerDay * 30),
                    _ => new TimeSpan(sign * num * TimeSpan.TicksPerDay * 365)
                };
            }
            return span;
        }

        private static Unit GetUnit(string s)
        {
            if (s.StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
                return Unit.Seconds;
            if (s.StartsWith("min", StringComparison.InvariantCultureIgnoreCase))
                return Unit.Minutes;
            if (s.StartsWith("h", StringComparison.InvariantCultureIgnoreCase))
                return Unit.Hours;
            if (s.StartsWith("d", StringComparison.InvariantCultureIgnoreCase))
                return Unit.Days;
            if (s.StartsWith("w", StringComparison.InvariantCultureIgnoreCase))
                return Unit.Weeks;
            if (s.StartsWith("mon", StringComparison.InvariantCultureIgnoreCase))
                return Unit.Months;
            if (s.StartsWith("y", StringComparison.InvariantCultureIgnoreCase))
                return Unit.Years;

            throw new ArgumentException($"Invalid offset unit '{s}'");
        }
    }
}