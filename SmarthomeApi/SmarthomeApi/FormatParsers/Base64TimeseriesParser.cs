using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SmarthomeApi.FormatParsers
{
    public class TimeSeries<T> : IEnumerable<KeyValuePair<DateTime, T?>> where T : struct
    {
        private List<KeyValuePair<DateTime, T?>> _list;
        private DateTime _begin;
        private DateTime _end;
        private int _count;

        /// <summary>
        /// Creates a new TimeSeries object with the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeries(DateTime begin, DateTime end, int count)
        {
            if (begin >= end)
                throw new ArgumentException("begin after end");

            _begin = begin;
            _end = end;
            _count = count;

            var duration = end.Subtract(begin).Divide(count);
            _list = Enumerable.Range(0, count).Select(t => 
                new KeyValuePair<DateTime, T?>(begin.Add(duration * t), default(T?))).ToList();
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
        public T? this[DateTime time]
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
            var bucket = timeBucket = _list.SkipWhile(d => d.Key < time).FirstOrDefault().Key;
            var foundIndex = _list.FindIndex(li => li.Key == bucket);
            index = Math.Max(0, foundIndex - 1);
            return foundIndex >= 0;
        }

        /// <summary>
        /// Sets or gets the given value in the time bucket with the given index.
        /// </summary>
        public T? this[int index]
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

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public IEnumerator<KeyValuePair<DateTime, T?>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }

    public static class Base64TimeseriesParser
    {
        private const int decimalPlaces = 1;
        
        public static TimeSeries<T> ToTimeseries<T>(this string base64, DateTime begin, DateTime end, int count) where T : struct
        {
            var timeseries = new TimeSeries<T>(begin, end, count);
            if (string.IsNullOrWhiteSpace(base64))
                return timeseries;

            var bytes = Convert.FromBase64String(base64);
            if (typeof(T) == typeof(bool))
            {
                int i = 0;
                var boolValues = bytes.SelectMany(d => new byte[] { (byte)((d >> 6) & 0x3),
                    (byte)((d >> 4) & 0x3), (byte)((d >> 2) & 0x3), (byte)(d & 0x3) })
                    .Select(d => d > 0x01 ? null : (bool?)(d == 0x01)).Take(count).ToList();
                foreach (var b in boolValues)
                {
                    (timeseries as TimeSeries<bool>)[i++] = b;
                }
                return timeseries;
            }
            
            for (int i = 0; i < bytes.Length - 1 && i < count * 2; i += 2)
            {
                var value = (short)(bytes[i] << 8 | bytes[i + 1]);
                if (typeof(T) == typeof(int))
                    (timeseries as TimeSeries<int>)[i / 2] = value == short.MinValue ? null : (short?)value;
                else if (typeof(T) == typeof(double))
                    (timeseries as TimeSeries<double>)[i / 2] = value == short.MinValue ? null : (double?)value / Math.Pow(10d, decimalPlaces);
            }

            return timeseries;
        }

        public static string ToBase64(this TimeSeries<double> timeseries)
        {
            return ToBase64(timeseries.Select(d => d.Value.HasValue ? 
                (int)(d.Value.Value * Math.Pow(10d, decimalPlaces)) : (int?)null));
        }

        public static string ToBase64(this TimeSeries<int> timeseries)
        {
            return ToBase64(timeseries.Select(d => d.Value));
        }

        public static string ToBase64(this TimeSeries<bool> timeseries)
        {
            var array = new byte[timeseries.Count / 4 + (timeseries.Count % 4 != 0 ? 1 : 0)];
            Func<bool?, int, byte> packNullableBool = (b, shift) => (byte)((!b.HasValue ? 0x03 : b.Value ? 0x01 : 0x00) << shift);
            Func<int, bool?> tryGetTsBool = tsIndex => tsIndex < timeseries.Count ? timeseries[tsIndex] : null;
            Func<int, int, byte> pack = (index, offset) => packNullableBool(tryGetTsBool(index * 4 + offset), 8 - ((offset + 1) * 2));
            for (int i = 0; i < array.Length; i++)
                array[i] = (byte)(pack(i, 0) | pack(i, 1) | pack(i, 2) | pack(i, 3));

            return Convert.ToBase64String(array);
        }

        private static string ToBase64(IEnumerable<int?> timeseries)
        {
            var array = timeseries.Select(d => d.HasValue ?
                Math.Min(short.MaxValue, Math.Max(short.MinValue - 1, (short)d.Value)) : short.MinValue)
                .SelectMany(d => new byte[] { (byte)(d >> 8), (byte)(d & 0xff) }).ToArray();

            return Convert.ToBase64String(array);
        }
    }
}
