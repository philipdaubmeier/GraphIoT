using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CompactTimeSeries
{
    public abstract class TimeSeriesBase<T> : ITimeSeries<T> where T : struct
    {
        protected TimeSeriesSpan _span;

        /// <summary>
        /// TimeSeriesBase constructor for the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeriesBase(TimeSeriesSpan span)
        {
            _span = span;
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.Trimmed()"/>
        /// </summary>
        public List<T?> Trimmed()
        {
            return this.SkipWhile(x => !x.Value.HasValue)
                .Reverse().SkipWhile(x => !x.Value.HasValue).Reverse()
                .Select(x => x.Value).ToList();
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.Trimmed(T)"/>
        /// </summary>
        public List<T> Trimmed(T defaultValue)
        {
            return Trimmed().Select(x => x ?? defaultValue).ToList();
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
        /// See <see cref="ITimeSeries{T}.Span"/>
        /// </summary>
        public TimeSeriesSpan Span => _span;

        /// <summary>
        /// See <see cref="ITimeSeries{T}.End"/>
        /// </summary>
        public DateTime Begin => _span.Begin;

        /// <summary>
        /// See <see cref="ITimeSeries{T}.End"/>
        /// </summary>
        public DateTime End => _span.End;

        /// <summary>
        /// See <see cref="ITimeSeries{T}.Count"/>
        /// </summary>
        public int Count => _span.Count;
        
        public abstract IEnumerator<KeyValuePair<DateTime, T?>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}