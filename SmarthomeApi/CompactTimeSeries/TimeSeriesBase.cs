using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CompactTimeSeries
{
    public abstract class TimeSeriesBase<T> : ITimeSeries<T> where T : struct
    {
        protected DateTime _begin;
        protected DateTime _end;
        protected int _count;
        protected TimeSpan _duration;

        /// <summary>
        /// TimeSeriesBase constructor for the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeriesBase(DateTime begin, DateTime end, int count)
        {
            if (begin >= end)
                throw new ArgumentException("begin after end");

            _begin = begin;
            _end = end;
            _count = count;
            _duration = end.Subtract(begin).Divide(count);
        }

        /// <summary>
        /// Extracts the maximum filled list, i.e. removes all items with
        /// null value from both the front and back of the enumeration of
        /// this time series. If any null values should be present between
        /// values, it will be set to the given defaultValue.
        /// </summary>
        public List<T> ToList(T defaultValue)
        {
            return this.SkipWhile(x => !x.Value.HasValue)
                .Reverse().SkipWhile(x => !x.Value.HasValue).Reverse()
                .Select(x => x.Value ?? defaultValue).ToList();
        }

        /// <summary>
        /// Sets the given value in the matching time bucket or sums the
        /// value up with the already existing value.
        /// </summary>
        public void Accumulate(DateTime time, T item)
        {
            var old = this[time];
            if (old.HasValue)
                this[time] = Add(old.Value, item);
            else
                this[time] = item;
        }

        private static ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
        private static ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
        private static Func<T, T, T> Add = Expression.Lambda<Func<T, T, T>>(Expression.Add(paramA, paramB), paramA, paramB).Compile();

        /// <summary>
        /// Sets or gets the given value in the matching time bucket.
        /// </summary>
        public abstract T? this[DateTime time] { set; get; }

        /// <summary>
        /// Sets or gets the given value in the time bucket with the given index.
        /// </summary>
        public abstract T? this[int index] { get; set; }

        /// <summary>
        /// Returns the begin date and time of this time series
        /// </summary>
        public DateTime Begin => _begin;

        /// <summary>
        /// Returns the end date and time of this time series
        /// </summary>
        public DateTime End => _end;

        /// <summary>
        /// Returns the number of elements (or time buckets, respectively) in this time series object.
        /// </summary>
        public int Count => _count;
        
        public abstract IEnumerator<KeyValuePair<DateTime, T?>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}