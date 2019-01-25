using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SmarthomeApi.FormatParsers.Utility
{
    public class CalendarWeekUtility
    {
        /// <summary>
        /// Returns the calendar week of the given date. The first week of the year
        /// is the first 4 day week, beginning with Monday.
        /// 
        /// This method is borrowed from http://stackoverflow.com/a/11155102/284240
        /// </summary>
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Returns the date of Monday in the given calendar week of the given year.
        /// 
        /// This method is borrowed from https://stackoverflow.com/a/19901870/210114
        /// </summary>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear, CultureInfo ci = null)
        {
            if (ci == null)
                ci = CultureInfo.InvariantCulture;

            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);

            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
                weekOfYear -= 1;

            return firstWeekDay.AddDays(weekOfYear * 7);
        }
    }
}
