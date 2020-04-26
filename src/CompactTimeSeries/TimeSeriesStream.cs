using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhilipDaubmeier.CompactTimeSeries
{
    public class TimeSeriesStream<T> : TimeSeriesBase<T>, IDisposable where T : struct
    {
        /// <summary>
        /// Private helper class for creating KeyValuePair structs that inherit the type parameter
        /// from the time series, but construct them from a known number type. For instance,
        /// if you have a int value you can neither construct nor cast it to a generic type
        /// parameter T and also not build a KeyValuePair from it. This factory, however, can
        /// be created with type parmeter T and then be casted to a concrete type.
        /// 
        /// Example usage:
        ///     var factory = new KeyValuePairFactory<T>();
        ///     int value = 23;
        ///     (factory as KeyValuePairFactory<int>).Set(time, value);
        ///     KeyValuePair<DateTime, T?> result = factory.Create();
        /// </summary>
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

        private readonly KeyValuePairFactory<T> _keyValuePairFactory = new KeyValuePairFactory<T>();

        private readonly Stream _stream;
        private readonly bool _isStreamManaged = false;
        private readonly int _startPosition = 0;

        private readonly int _decimalPlaces;

        /// <summary>
        /// Creates a new TimeSeriesStream object with the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeriesStream(TimeSeriesSpan span) : this(span, 1) { }

        /// <summary>
        /// Creates a new TimeSeriesStream object with the given number of equally spaced time buckets.
        /// </summary>
        public TimeSeriesStream(TimeSeriesSpan span, int decimalPlaces)
            : this(new MemoryStream(typeof(T) == typeof(bool) ? span.Count / 4 : span.Count * sizeof(short)), 0, span, decimalPlaces)
        {
            _isStreamManaged = true;

            for (int i = 0; i < _span.Count; i++)
                WriteValue(null);
        }

        /// <summary>
        /// Creates a new TimeSeriesStream object with an existing underlying stream that has to be
        /// readable, writable and seekable. The time series starts at the given streamPosition.
        /// </summary>
        public TimeSeriesStream(Stream stream, int streamPosition, TimeSeriesSpan span, int decimalPlaces = 1)
            : base(span)
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
        /// See <see cref="ITimeSeries{T}.this[DateTime time]"/>
        /// </summary>
        public override T? this[DateTime time]
        {
            get => !SeekToTimeBucket(time) ? (default) : ReadKeyValuePair(time).Value;
            set
            {
                if (SeekToTimeBucket(time))
                    WriteValue(value);
            }
        }

        /// <summary>
        /// See <see cref="ITimeSeries{T}.this[int index]"/>
        /// </summary>
        public override T? this[int index]
        {
            set
            {
                if (index < 0 || index >= _span.Count)
                    throw new ArgumentOutOfRangeException();

                if (!SeekToTimeBucket(index))
                    return;

                WriteValue(value);
            }
            get
            {
                if (index < 0 || index >= _span.Count)
                    throw new ArgumentOutOfRangeException();

                if (!SeekToTimeBucket(index))
                    return default;

                return ReadKeyValuePair(DateTime.MinValue).Value;
            }
        }

        public override IEnumerator<KeyValuePair<DateTime, T?>> GetEnumerator()
        {
            _stream.Seek(_startPosition, SeekOrigin.Begin);
            var time = _span.Begin;

            if (typeof(T) == typeof(bool))
            {
                for (int i = 0; i < _span.Count / 4; i++)
                {
                    foreach (var item in ReadBoolKeyValuePairs(time))
                        yield return item;

                    time += _span.Duration * 4;
                }
                yield break;
            }

            for (int i = 0; i < _span.Count; i++)
            {
                var item = ReadKeyValuePair(time);
                if (item.Key == DateTime.MinValue)
                    yield break;

                yield return item;
                time += _span.Duration;
            }
        }

        /// <summary>
        /// Seeks the stream position to the bucket for the given time. If the time
        /// is out of range, the current stream position is not touched and the function
        /// returns false.
        /// </summary>
        private bool SeekToTimeBucket(DateTime time)
        {
            if (time < _span.Begin || time > _span.End)
                return false;

            var index = (int)Math.Floor((time - _span.Begin) / _span.Duration);
            if (index >= _span.Count && time <= _span.End)
                index = _span.Count - 1;
            return SeekToTimeBucket(index);
        }

        /// <summary>
        /// Seeks the stream position to the bucket with the given index. If the index
        /// is out of range, the current stream position is not touched and the function
        /// returns false.
        /// </summary>
        private bool SeekToTimeBucket(int index)
        {
            var pos = _startPosition + index * sizeof(short);
            if (pos < 0 || pos > _stream.Length)
                return false;

            _stream.Seek(pos, SeekOrigin.Begin);
            return true;
        }

        /// <summary>
        /// Writes the given value to the stream. The given integer value is cropped
        /// to two bytes, i.e. the value range from short.MinValue + 1 to short.MaxValue,
        /// where the value short.MinValue is specially reserved for representing null
        /// in case of the nullable value is null. If a floating point type is given in
        /// the generic type parameter, it is first converted to a fixed point decimal
        /// with the number of decimal places given in the constructor of this object.
        /// The value range then decreases depending on the number of decimal places,
        /// e.g. -32767 to 32767 for 0 decimal places, -3276.7 to 3276.7 for 1 decimal places,
        /// -0.32767 to 0.32767 for 5 decimal places etc. Positive and negative infinities
        /// are cropped to max and min values respectively, NaN values are treated as null.
        /// </summary>
        private void WriteValue(T? value)
        {
            if (typeof(T) != typeof(int) && typeof(T) != typeof(double))
                throw new InvalidOperationException("TimeSeriesStream currently only supports writing int and double");

            int? val = 0;
            if (typeof(T) == typeof(int))
                val = (value as int?);
            else if (typeof(T) == typeof(double))
            {
                double? doubleval = value as double?;
                if (!doubleval.HasValue || double.IsNaN(doubleval.Value))
                    val = null;
                else
                {
                    var uncropped = doubleval.Value * Math.Pow(10d, _decimalPlaces);
                    val = uncropped > int.MaxValue ? int.MaxValue : uncropped < int.MinValue ? int.MinValue : (int)uncropped;
                }
            }

            short valShort = val.HasValue ? (short)Math.Min(short.MaxValue, Math.Max(short.MinValue + 1, val.Value)) : short.MinValue;
            _stream.WriteByte((byte)(valShort >> 8));
            _stream.WriteByte((byte)(valShort & 0xff));
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
                (_keyValuePairFactory as KeyValuePairFactory<bool>)!.Set(time, b);
                yield return _keyValuePairFactory.Create();

                time += _span.Duration;
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
                return new KeyValuePair<DateTime, T?>(DateTime.MinValue, default);

            var value = (short)(readByte1 << 8 | readByte2);

            if (typeof(T) == typeof(int))
                (_keyValuePairFactory as KeyValuePairFactory<int>)
                    !.Set(time, value == short.MinValue ? null : (short?)value);
            else if (typeof(T) == typeof(double))
                (_keyValuePairFactory as KeyValuePairFactory<double>)
                    !.Set(time, value == short.MinValue ? null : (double?)value / Math.Pow(10d, _decimalPlaces));

            return _keyValuePairFactory.Create();
        }
    }
}