using System;
using System.Linq;
using System.Linq.Expressions;

namespace PhilipDaubmeier.CompactTimeSeries
{
    public class TimeSeriesCompressor<TKey, T> where T : struct
    {
        private readonly TimeSeriesStreamCollection<TKey, T> _timeseries;

        /// <summary>
        /// Creates a new instance of the TimeSeriesCompressor class for converting the given
        /// TimeSeriesStreamCollection into a diff-compacted form before gzip compression.
        /// Both compaction effects add up as the diff-ing of mostly constant or steady series
        /// results in many small and similar values which can then be huffman coded more efficiently.
        /// </summary>
        public TimeSeriesCompressor(TimeSeriesStreamCollection<TKey, T> timeseries) => _timeseries = timeseries;

        public void Dispose() => _timeseries.Dispose();

        public byte[] ToCompressedByteArray()
        {
            using var clonedTimeSeries = CloneTimeSeriesKeys();
            foreach (var key in _timeseries.Select(x => x.Key))
                WriteDiffTimeseries(_timeseries[key], clonedTimeSeries[key]);

            return (clonedTimeSeries.UnderlyingStream as CompressableMemoryStream)?.ToCompressedByteArray();
        }

        private TimeSeriesStreamCollection<TKey, T> CloneTimeSeriesKeys()
        {
            var metr = _timeseries.Metrics;
            var firstitem = (_timeseries.FirstOrDefault().Value as TimeSeriesBase<T>);
            if (firstitem == null)
                return null;

            var clonedTimeSeries = new TimeSeriesStreamCollection<TKey, T>(_timeseries.Select(x => x.Key),
                metr.TimeseriesOffset, (k, s) => { s.Position += metr.TimeseriesOffset; }, firstitem.Span, metr.DecimalPlaces);

            foreach (var pos in Enumerable.Range(0, metr.TimeseriesBucketCount)
                .Select(x => metr.CollectionCountOffset + x * metr.TimeseriesSize))
            {
                _timeseries.UnderlyingStream.Position = pos;
                clonedTimeSeries.UnderlyingStream.Position = pos;

                for (int i = 0; i < metr.TimeseriesOffset; i++)
                    clonedTimeSeries.UnderlyingStream.WriteByte((byte)_timeseries.UnderlyingStream.ReadByte());
            }
            return clonedTimeSeries;
        }

        /// <summary>
        /// Writes the given source TimeSeries<T> to the target as diff series, i.e. the first
        /// value is encoded directly, all following values are encoded as diffs to previous
        /// values. If any value is null (TimeSeries<T> contains nullable T), it is treated
        /// as value 0 for diff calculation but put into the stream as short.MinValue to be
        /// losslessly restoreable. Important: this encoding is only lossless for values in the
        /// interval -16383 (short.MinValue + 1 / 2) and 16383 (short.MaxValue / 2) inclusively.
        /// All values smaller or greater than those get cropped because they could lead to diffs
        /// outside the signed 16 bit (short) value range.
        /// 
        /// Example:
        ///  - input values (nullable int in source TimeSeries object):
        ///     [120, 123, 120, 121, 123, 123, 120, null, null, 121, 124, 127, 1030,  127]
        ///     
        ///  - output values (in dest TimeSeries object), note the first value remains untouched:
        ///     [120,   3,  -3,   1,   2,   0,  -3, null, null,   1,   3,   3,  903, -903]
        /// </summary>
        private void WriteDiffTimeseries(ITimeSeries<T> source, ITimeSeries<T> dest)
        {
            bool first = true;
            T previous = default;
            for (int i = 0; i < source.Count; i++)
            {
                T current = source[i] ?? previous;
                T diff = GenericSubtract(current, previous);
                previous = current;

                if (first)
                {
                    diff = source[i] ?? default;
                    first = false;
                }

                dest[i] = diff;
            }
        }

        private static readonly ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
        private static readonly ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
        private static readonly Func<T, T, T> GenericSubtract = Expression.Lambda<Func<T, T, T>>(Expression.Subtract(paramA, paramB), paramA, paramB).Compile();
    }
}