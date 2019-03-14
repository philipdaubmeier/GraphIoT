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
        public TimeSeries(DateTime begin, DateTime end, int count)
            : base(begin, end, count)
        {
            _list = Enumerable.Range(0, count).Select(t =>
                new KeyValuePair<DateTime, T?>(begin.Add(_duration * t), default(T?))).ToList();
        }

        /// <summary>
        /// Sets or gets the given value in the matching time bucket.
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

        private bool TryFindIndex(DateTime time, out DateTime timeBucket, out int index)
        {
            var bucket = _list.SkipWhile(d => d.Key <= time).FirstOrDefault().Key;
            var foundIndex = time < _begin ? -1
                : bucket == DateTime.MinValue && time <= _end ? _list.Count
                : _list.FindIndex(li => li.Key == bucket);
            index = Math.Max(0, foundIndex - 1);
            timeBucket = index < _list.Count ? _list[index].Key : DateTime.MinValue;
            return foundIndex >= 0 && timeBucket != DateTime.MinValue;
        }

        /// <summary>
        /// Sets or gets the given value in the time bucket with the given index.
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
    }
}
