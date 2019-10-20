using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.CompactTimeSeries
{
    public class TimeSeries<T> : TimeSeriesBase<T> where T : struct
    {
        private readonly List<KeyValuePair<DateTime, T?>> _list;

        /// <summary>
        /// Creates a new TimeSeries object with the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeries(TimeSeriesSpan span)
            : base(span)
        {
            _list = Enumerable.Range(0, _span.Count).Select(t =>
                new KeyValuePair<DateTime, T?>(span.Begin.Add(_span.Duration * t), default)).ToList();
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.this[DateTime time]"/>
        /// </summary>
        public override T? this[DateTime time]
        {
            get => !TryFindIndex(time, out _, out int index) ? default : _list[index].Value;
            set
            {
                if (TryFindIndex(time, out DateTime timeBucket, out int index))
                    _list[index] = new KeyValuePair<DateTime, T?>(timeBucket, value);
            }
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.this[int index]"/>
        /// </summary>
        public override T? this[int index]
        {
            get => _list[index].Value;
            set => _list[index] = new KeyValuePair<DateTime, T?>(_list[index].Key, value);
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
