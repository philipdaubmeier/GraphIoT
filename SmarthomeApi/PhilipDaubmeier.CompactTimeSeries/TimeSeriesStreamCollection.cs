using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhilipDaubmeier.CompactTimeSeries
{
    public class TimeSeriesStreamCollection<TKey, T> : IEnumerable<KeyValuePair<TKey, ITimeSeries<T>>>, IDisposable where T : struct
    {
        private Dictionary<TKey, ITimeSeries<T>> _dict;
        private CompressableMemoryStream _stream;

        public class BinaryStreamMetrics
        {
            public BinaryStreamMetrics(int keySize, int bucketCount, int decimalPlaces)
            {
                TimeseriesOffset = keySize;
                TimeseriesBucketCount = bucketCount;
                DecimalPlaces = decimalPlaces;
            }

            public int CollectionCountOffset => sizeof(int);
            public int TimeseriesOffset { get; private set; }
            public int TimeseriesBucketCount { get; private set; }
            public int TimeseriesSize => TimeseriesBucketCount * sizeof(short) + TimeseriesOffset;
            public int DecimalPlaces { get; private set; }

            public int StreamSize(int keyCount) => (keyCount * TimeseriesSize) + CollectionCountOffset;

            public int KeyPosition(int index) => CollectionCountOffset + index * TimeseriesSize;
            public int TimeseriesPosition(int index) => TimeseriesOffset + KeyPosition(index);
        }

        public BinaryStreamMetrics Metrics { get; private set; }

        /// <summary>
        /// Creates a new empty TimeSeriesStreamCollection object. It initializes an underlying MemoryStream and
        /// inits it with default values for all TimeSeries (one for each given key is created). Every write access
        /// to any TimeSeries will then be in-place on the already allocated buffer with the fixed size of
        /// keys.Count * bucketCount.
        /// </summary>
        public TimeSeriesStreamCollection(IEnumerable<TKey> keys, int keySize, Action<TKey, Stream> writeKeyAction, TimeSeriesSpan span, int decimalPlaces = 1) : base()
        {
            var keyList = keys.ToList();
            Metrics = new BinaryStreamMetrics(keySize, span.Count, decimalPlaces);

            _dict = new Dictionary<TKey, ITimeSeries<T>>();

            _stream = new CompressableMemoryStream(Metrics.StreamSize(keyList.Count));            
            _stream.SetLength(Metrics.StreamSize(keyList.Count));

            using (var writer = new BinaryWriter(_stream, System.Text.Encoding.UTF8, true))
                writer.Write(keyList.Count);

            foreach (var i in Enumerable.Range(0, keyList.Count))
            {
                writeKeyAction(keyList[i], _stream);

                var timeseries = new TimeSeriesStream<T>(_stream, Metrics.TimeseriesPosition(i), span, decimalPlaces);
                foreach (var item in timeseries)
                    timeseries[item.Key] = null;

                _dict.Add(keyList[i], timeseries);
            }
        }

        /// <summary>
        /// Restores a TimeSeriesStreamCollection from a compressed byte array.
        /// </summary>
        public TimeSeriesStreamCollection(byte[] compressedByteArray, int keySize, Func<Stream, TKey> readKeyFunc, TimeSeriesSpan span, int decimalPlaces = 1) : base()
        {
            Metrics = new BinaryStreamMetrics(keySize, span.Count, decimalPlaces);
            _stream = CompressableMemoryStream.FromCompressedByteArray(compressedByteArray);

            _dict = new Dictionary<TKey, ITimeSeries<T>>();

            int count = 0;
            using (var reader = new BinaryReader(_stream, System.Text.Encoding.UTF8, true))
                count = reader.ReadInt32();

            foreach (var i in Enumerable.Range(0, count))
            {
                _stream.Seek(Metrics.KeyPosition(i), SeekOrigin.Begin);
                var key = readKeyFunc(_stream);

                _dict.Add(key, new TimeSeriesStream<T>(_stream, Metrics.TimeseriesPosition(i), span, decimalPlaces));
            }
        }

        public ITimeSeries<T> this[TKey key] => _dict[key];

        public IEnumerator<KeyValuePair<TKey, ITimeSeries<T>>> GetEnumerator() => _dict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

        public Stream UnderlyingStream => _stream;

        public void Dispose()
        {
            _stream.Dispose();
        }

        public override string ToString()
        {
            var timeseriesStrings = _dict.Select(x => $"'{x.Key}': " + string.Join("; ", x.Value.Select(y => $"{y.Key}: {y.Value?.ToString() ?? "null"}")));
            var dictStr = string.Join(", ", timeseriesStrings.Select(x => $"{{{x}}}"));
            return $"{_dict.Count}: {dictStr}";
        }

        /// <summary>
        /// Returns a GZipped byte array of the contents of this TimeSeriesStreamCollection.
        /// </summary>
        public byte[] ToCompressedByteArray() => _stream.ToCompressedByteArray();
    }
}