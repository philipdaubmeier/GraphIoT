using System;
using System.Collections.Generic;

namespace CompactTimeSeries
{
    public class TimeSeriesSpan
    {
        /// <summary>
        /// Returns the begin date and time of this time series span
        /// </summary>
        public DateTime Begin { get; private set; }

        /// <summary>
        /// Returns the end date and time of this time series span
        /// </summary>
        public DateTime End { get; private set; }

        /// <summary>
        /// Returns the number of elements (or time buckets, respectively) in this time series span object.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Returns the timespan of a time bucket of this object.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Enum of commonly used time bucket spacings for a time series
        /// </summary>
        public enum Spacing
        {
            Spacing1Sec = 1,
            Spacing5Sec = 5,
            Spacing1Min = 60,
            Spacing5Min = 5 * 60,
            Spacing10Min = 10 * 60,
            Spacing30Min = 30 * 60,
            Spacing1Hour = 60 * 60,
            Spacing1Day = 24 * 60 * 60
        }

        /// <summary>
        /// Creates a new TimeSeriesSpan object with the given spacing and number of equally spaced time buckets.
        /// </summary>
        public TimeSeriesSpan(DateTime begin, Spacing spacing, int count)
            : this(begin, begin.AddSeconds((double)count * (int)spacing), count)
        { }

        /// <summary>
        /// Creates a new TimeSeriesSpan object with the given time period and spacing of time buckets.
        /// </summary>
        public TimeSeriesSpan(DateTime begin, DateTime end, Spacing spacing)
            : this(begin, end, (int)((end - begin).TotalSeconds / (int)spacing))
        { }

        /// <summary>
        /// Creates a new TimeSeriesSpan object with the given time period and a number of equally spaced time buckets.
        /// </summary>
        public TimeSeriesSpan(DateTime begin, DateTime end, int count)
        {
            if (begin > end)
            {
                Begin = end;
                End = begin;
            }
            else
            {
                Begin = begin;
                End = end;
            }

            Count = count;
            Duration = End.Subtract(Begin).Divide(Count);
        }

        /// <summary>
        /// Returns all dates (i.e. no time component) that are contained in this
        /// time series time interval.
        /// </summary>
        public IEnumerable<DateTime> IncludedDates()
        {
            DateTime d1 = Begin.Date, d2 = End.Date;
            if (d1 >= d2)
            {
                yield return d1;
                yield break;
            }

            for (int i = 0; i <= (d2 - d1).Days; i++)
                yield return d1.AddDays(i);
        }
    }
}