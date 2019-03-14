using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompactTimeSeries
{
    public class TimeSeriesStream<T> : TimeSeriesBase<T>, IDisposable where T : struct
    {
        private class KeyValuePairFactory<Tfac> where Tfac : struct
        {
            private DateTime _time;
            private Tfac? _value;

            public void Set(DateTime time, Tfac? value)
            {
                _time = time;
                _value = value;
            }

            public KeyValuePair<DateTime, Tfac?> Create()
            {
                return new KeyValuePair<DateTime, Tfac?>(_time, _value);
            }
        }

        private KeyValuePairFactory<T> _keyValuePairFactory = new KeyValuePairFactory<T>();

        private Stream _stream;
        private bool _isStreamManaged = false;
        private int _startPosition = 0;
        
        private int _decimalPlaces;
        
        /// <summary>
        /// Creates a new TimeSeriesStream object with the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeriesStream(DateTime begin, DateTime end, int count, int decimalPlaces = 1)
            : this(new MemoryStream(typeof(T) == typeof(bool) ? count / 4 : count * sizeof(short)), 0, begin, end, count, decimalPlaces)
        {
            _isStreamManaged = true;

            for (int i = 0; i < count; i++)
                WriteValue(null);
        }

        /// <summary>
        /// Creates a new TimeSeriesStream object with an existing underlying stream that has to be
        /// readable, writable and seekable. The time series starts at the given streamPosition.
        /// </summary>
        public TimeSeriesStream(Stream stream, int streamPosition, DateTime begin, DateTime end, int count, int decimalPlaces = 1)
            : base (begin, end, count)
        {
            if (stream == null)
                throw new ArgumentException("underlying stream can not be null");

            if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
                throw new ArgumentException("underlying stream must be readable, writable and seekable");
            
            _stream = stream;
            _startPosition = streamPosition;
            _decimalPlaces = decimalPlaces;
        }
        
        public void Dispose()
        {
            if (_isStreamManaged)
                _stream.Dispose();
        }

        /// <summary>
        /// Sets or gets the given value in the matching time bucket.
        /// </summary>
        public override T? this[DateTime time]
        {
            set
            {
                if (!SeekToTimeBucket(time))
                    return;

                WriteValue(value);
            }
            get
            {
                if (!SeekToTimeBucket(time))
                    return default(T?);

                return ReadKeyValuePair(time).Value;
            }
        }

        /// <summary>
        /// Sets or gets the given value in the time bucket with the given index.
        /// </summary>
        public override T? this[int index]
        {
            set
            {
                if (!SeekToTimeBucket(index))
                    return;

                WriteValue(value);
            }
            get
            {
                if (!SeekToTimeBucket(index))
                    return default(T?);

                return ReadKeyValuePair(DateTime.MinValue).Value;
            }
        }

        private bool SeekToTimeBucket(DateTime time)
        {
            if (time < _begin || time > _end)
                return false;

            var index = (int)Math.Floor((time - _begin) / _duration);
            return SeekToTimeBucket(index);
        }

        private bool SeekToTimeBucket(int index)
        {
            var pos = _startPosition + index * sizeof(short);
            if (pos < 0 || pos > _stream.Length)
                return false;

            _stream.Seek(pos, SeekOrigin.Begin);
            return true;
        }

        private void WriteValue(T? value)
        {
            if (typeof(T) != typeof(int) && typeof(T) != typeof(double))
                throw new InvalidOperationException("TimeSeriesStream currently only supports writing int and double");

            int? val = 0;
            if (typeof(T) == typeof(int))
                val = (value as int?);
            else if (typeof(T) == typeof(double))
                val = value.HasValue ? (int)((value as double?).Value * Math.Pow(10d, _decimalPlaces)) : (int?)null;
            
            short valShort = val.HasValue ? (short)Math.Min(short.MaxValue, Math.Max(short.MinValue - 1, val.Value)) : short.MinValue;
            _stream.WriteByte((byte)(valShort >> 8));
            _stream.WriteByte((byte)(valShort & 0xff));
        }

        public override IEnumerator<KeyValuePair<DateTime, T?>> GetEnumerator()
        {
            _stream.Seek(_startPosition, SeekOrigin.Begin);
            var time = _begin;

            if (typeof(T) == typeof(bool))
            {
                for (int i = 0; i < _count / 4; i++)
                {
                    foreach (var item in ReadBoolKeyValuePairs(time))
                        yield return item;

                    time += _duration * 4;
                }
                yield break;
            }
            
            for (int i = 0; i < _count; i++)
            {
                var item = ReadKeyValuePair(time);
                if (item.Key == DateTime.MinValue)
                    yield break;

                yield return item;
                time += _duration;
            }
        }

        /// <summary>
        /// Reads the four nullable bool values on the current byte position of the stream.
        /// If the stream reached the end, it returns zero items. The first bool value
        /// will get the given time value, the next one the next time bucket and so on.
        /// </summary>
        private IEnumerable<KeyValuePair<DateTime, T?>> ReadBoolKeyValuePairs(DateTime time)
        {
            int readByte = _stream.ReadByte();
            if (readByte < 0)
                yield break;

            var boolValues = (new byte[] {
                        (byte)((readByte >> 6) & 0x3), (byte)((readByte >> 4) & 0x3),
                        (byte)((readByte >> 2) & 0x3), (byte)(readByte & 0x3) })
                .Select(d => d > 0x01 ? null : (bool?)(d == 0x01)).ToList();
            foreach (var b in boolValues)
            {
                (_keyValuePairFactory as KeyValuePairFactory<bool>).Set(time, b);
                yield return _keyValuePairFactory.Create();

                time += _duration;
            }
        }

        /// <summary>
        /// Reads the int or double value on the current byte position of the stream.
        /// </summary>
        private KeyValuePair<DateTime, T?> ReadKeyValuePair(DateTime time)
        {
            int readByte1 = _stream.ReadByte();
            int readByte2 = _stream.ReadByte();
            if (readByte1 < 0 || readByte2 < 0)
                return new KeyValuePair<DateTime, T?>(DateTime.MinValue, default(T?));

            var value = (short)(readByte1 << 8 | readByte2);

            if (typeof(T) == typeof(int))
                (_keyValuePairFactory as KeyValuePairFactory<int>)
                    .Set(time, value == short.MinValue ? null : (short?)value);
            else if (typeof(T) == typeof(double))
                (_keyValuePairFactory as KeyValuePairFactory<double>)
                    .Set(time, value == short.MinValue ? null : (double?)value / Math.Pow(10d, _decimalPlaces));

            return _keyValuePairFactory.Create();
        }
    }
}