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
        /// See <see cref="ITimeSeries{T}.ToList(T)"/>
        /// </summary>
        public List<T> ToList(T defaultValue)
        {
            return this.SkipWhile(x => !x.Value.HasValue)
                .Reverse().SkipWhile(x => !x.Value.HasValue).Reverse()
                .Select(x => x.Value ?? defaultValue).ToList();
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.Accumulate(DateTime, T)"/>
        /// </summary>
        public void Accumulate(DateTime time, T item)
        {
            var old = this[time];
            if (old.HasValue)
                this[time] = Add(old.Value, item);
            else
                this[time] = item;
        }

        // Helper method/hack for adding up generic values
        private static ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
        private static ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
        private static Func<T, T, T> Add = Expression.Lambda<Func<T, T, T>>(Expression.Add(paramA, paramB), paramA, paramB).Compile();

        /// <summary>
        /// See <see cref="ITimeSeries{T}.this[DateTime time]"/>
        /// </summary>
        public abstract T? this[DateTime time] { set; get; }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.this[int index]"/>
        /// </summary>
        public abstract T? this[int index] { get; set; }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.End"/>
        /// </summary>
        public DateTime Begin => _begin;

        /// <summary>
        /// See <see cref="ITimeSeries{T}.End"/>
        /// </summary>
        public DateTime End => _end;

        /// <summary>
        /// See <see cref="ITimeSeries{T}.Count"/>
        /// </summary>
        public int Count => _count;
        
        public abstract IEnumerator<KeyValuePair<DateTime, T?>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}