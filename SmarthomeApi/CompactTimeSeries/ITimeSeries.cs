using System;
using System.Collections.Generic;

namespace CompactTimeSeries
{
    public interface ITimeSeries<T> : IEnumerable<KeyValuePair<DateTime, T?>> where T : struct
    {
        /// <summary>
        /// Extracts the maximum filled list, i.e. removes all items with
        /// null value from both the front and back of the enumeration of
        /// this time series. If any null values should be present between
        /// values, it will be set to the given defaultValue.
        /// </summary>
        List<T> ToList(T defaultValue);

        /// <summary>
        /// Sets the given value in the matching time bucket or sums the
        /// value up with the already existing value.
        /// </summary>
        void Accumulate(DateTime time, T item);

        /// <summary>
        /// Sets or gets the given value in the matching time bucket.
        /// </summary>
        T? this[DateTime time] { set; get; }

        /// <summary>
        /// Sets or gets the given value in the time bucket with the given index.
        /// </summary>
        T? this[int index] { get; set; }

        /// <summary>
        /// Returns the time series span of this time series, i.e. an object holding start, end and time spacing
        /// </summary>
        TimeSeriesSpan Span { get; }

        /// <summary>
        /// Returns the begin date and time of this time series
        /// </summary>
        DateTime Begin { get; }

        /// <summary>
        /// Returns the end date and time of this time series
        /// </summary>
        DateTime End { get; }

        /// <summary>
        /// Returns the number of elements (or time buckets, respectively) in this time series object.
        /// </summary>
        int Count { get; }
    }
}