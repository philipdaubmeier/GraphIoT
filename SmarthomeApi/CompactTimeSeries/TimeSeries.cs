using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactTimeSeries
{
    public class TimeSeries<T> : TimeSeriesBase<T> where T : struct
    {
        private List<KeyValuePair<DateTime, T?>> _list;

        /// <summary>
        /// Creates a new TimeSeries object with the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeries(TimeSeriesSpan span)
            : base(span)
        {
            _list = Enumerable.Range(0, _span.Count).Select(t =>
                new KeyValuePair<DateTime, T?>(span.Begin.Add(_span.Duration * t), default(T?))).ToList();
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.this[DateTime time]"/>
        /// </summary>
        public override T? this[DateTime time]
        {
            set
            {
                if (!TryFindIndex(time, out DateTime timeBucket, out int index))
                    return;

                _list[index] = new KeyValuePair<DateTime, T?>(timeBucket, value);
            }
            get
            {
                if (!TryFindIndex(time, out DateTime timeBucket, out int index))
                    return default(T?);

                return _list[index].Value;
            }
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.this[int index]"/>
        /// </summary>
        public override T? this[int index]
        {
            set
            {
                _list[index] = new KeyValuePair<DateTime, T?>(_list[index].Key, value);
            }
            get
            {
                return _list[index].Value;
            }
        }

        public override IEnumerator<KeyValuePair<DateTime, T?>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Searches for the right time bucket for this given time and returns both the exact
        /// time bucket and the list index, if found. If the given time is out of range, the
        /// function returns false.
        /// </summary>
        private bool TryFindIndex(DateTime time, out DateTime timeBucket, out int index)
        {
            var bucket = _list.SkipWhile(d => d.Key <= time).FirstOrDefault().Key;
            var foundIndex = time < _span.Begin ? -1
                : bucket == DateTime.MinValue && time <= _span.End ? _list.Count
                : _list.FindIndex(li => li.Key == bucket);
            index = Math.Max(0, foundIndex - 1);
            timeBucket = index < _list.Count ? _list[index].Key : DateTime.MinValue;
            return foundIndex >= 0 && timeBucket != DateTime.MinValue;
        }
    }
}
