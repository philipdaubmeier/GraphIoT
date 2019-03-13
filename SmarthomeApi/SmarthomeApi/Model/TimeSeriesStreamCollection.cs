﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SmarthomeApi.Model
{
    public class TimeSeriesStreamCollection<TKey, T> : IEnumerable<KeyValuePair<TKey, ITimeSeries<T>>>, IDisposable where T : struct
    {
        private const double _compressionEstimate = 0.2d;

        private Dictionary<TKey, ITimeSeries<T>> _dict;
        private Stream _stream;

        private class BinaryStreamMetrics
        {
            private readonly int _bucketCount;

            public BinaryStreamMetrics(int keySize, int bucketCount)
            {
                TimeseriesOffset = keySize;
                _bucketCount = bucketCount;
            }

            public int CollectionCountOffset => sizeof(int);
            public int TimeseriesOffset { get; private set; }
            public int TimeseriesSize => _bucketCount * sizeof(short) + TimeseriesOffset;

            public int StreamSize(int keyCount) => (keyCount * TimeseriesSize) + CollectionCountOffset;

            public int TimeseriesPosition(int index) => CollectionCountOffset + TimeseriesOffset + index * TimeseriesSize;
        }

        /// <summary>
        /// Creates a new empty TimeSeriesStreamCollection object. It initializes an underlying MemoryStream and
        /// inits it with default values for all TimeSeries (one for each given key is created). Every write access
        /// to any TimeSeries will then be in-place on the already allocated buffer with the fixed size of
        /// keys.Count * bucketCount.
        /// </summary>
        public TimeSeriesStreamCollection(IEnumerable<TKey> keys, int keySize, Action<TKey, Stream> writeKeyAction, DateTime begin, DateTime end, int bucketCount, int decimalPlaces = 1) : base()
        {
            var keyList = keys.ToList();
            var metrics = new BinaryStreamMetrics(keySize, bucketCount);

            _dict = new Dictionary<TKey, ITimeSeries<T>>();

            _stream = new MemoryStream(metrics.StreamSize(keyList.Count));            
            _stream.SetLength(metrics.StreamSize(keyList.Count));

            using (var writer = new BinaryWriter(_stream, System.Text.Encoding.UTF8, true))
                writer.Write(keyList.Count);

            foreach (var i in Enumerable.Range(0, keyList.Count))
            {
                writeKeyAction(keyList[i], _stream);

                var timeseries = new TimeSeriesStream<T>(_stream, metrics.TimeseriesPosition(i), begin, end, bucketCount, decimalPlaces);
                foreach (var item in timeseries)
                    timeseries[item.Key] = null;

                _dict.Add(keyList[i], timeseries);
            }
        }

        /// <summary>
        /// Restores a TimeSeriesStreamCollection from a compressed byte array.
        /// </summary>
        public TimeSeriesStreamCollection(byte[] compressedByteArray, int keySize, Func<Stream, TKey> readKeyFunc, DateTime begin, DateTime end, int bucketCount, int decimalPlaces = 1) : base()
        {
            var metrics = new BinaryStreamMetrics(keySize, bucketCount);
            FromCompressedByteArray(compressedByteArray);

            _dict = new Dictionary<TKey, ITimeSeries<T>>();

            int count = 0;
            using (var reader = new BinaryReader(_stream, System.Text.Encoding.UTF8, true))
                count = reader.ReadInt32();

            foreach (var i in Enumerable.Range(0, count))
            {
                var key = readKeyFunc(_stream);
                _dict.Add(key, new TimeSeriesStream<T>(_stream, metrics.TimeseriesPosition(i), begin, end, bucketCount, decimalPlaces));
            }
        }

        public ITimeSeries<T> this[TKey key] => _dict[key];

        public IEnumerator<KeyValuePair<TKey, ITimeSeries<T>>> GetEnumerator() => _dict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

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
        public byte[] ToCompressedByteArray()
        {
            using (var compressedStream = new MemoryStream((int)(_stream.Length * _compressionEstimate)))
            using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
            {
                _stream.Position = 0;
                _stream.CopyTo(compressionStream);
                compressionStream.Close();
                return compressedStream.ToArray();
            }
        }

        /// <summary>
        /// Initializes the underlying MemoryStream from a GZipped byte array.
        /// </summary>
        private void FromCompressedByteArray(byte[] byteArray)
        {
            _stream = new MemoryStream((int)(byteArray.Length / _compressionEstimate));
            using (var compressedStream = new MemoryStream(byteArray, 0, byteArray.Length, false) { Position = 0 })
            using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(_stream);
                _stream.Position = 0;
            }
        }
    }
}