using PhilipDaubmeier.CompactTimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Core.Aggregation
{
    public static class ResamplerExtensions
    {
        public static void Aggregate<Tseries>(this TimeSeriesResampler<Tseries, double> resampler, ITimeSeries<double> series, Aggregator aggregator, double correctionFactor = 1d, double correctionOffset = 0d) where Tseries : TimeSeriesBase<double>
        {
            resampler.Aggregate(new List<ITimeSeries<double>>() { series }, aggregator, x => x, x => x, correctionFactor, correctionOffset);
        }

        public static void Aggregate<Tseries>(this TimeSeriesResampler<Tseries, double> resampler, IEnumerable<ITimeSeries<double>> series, Aggregator aggregator, double correctionFactor = 1d, double correctionOffset = 0d) where Tseries : TimeSeriesBase<double>
        {
            resampler.Aggregate(series, aggregator, x => x, x => x, correctionFactor, correctionOffset);
        }

        public static void Aggregate<Tseries, Tval>(this TimeSeriesResampler<Tseries, Tval> resampler, IEnumerable<ITimeSeries<Tval>> series, Aggregator aggregator, Func<Tval, double> selector, Func<double, Tval> resultCast, double correctionFactor = 1d, double correctionOffset = 0d) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            var resamplingFactor = Math.Max(1d, series.First().Span.Duration / resampler.Resampled.Span.Duration);

            Func<IEnumerable<Tval>, double> aggregationFunc = aggregator switch
            {
                var a when
                    a == Aggregator.Average ||
                    a == Aggregator.Default => x => x.Average(selector),
                Aggregator.AverageZero => x => x.Sum(selector) / resamplingFactor,
                Aggregator.Median => x => x.Median(selector),
                Aggregator.Sum => x => x.Sum(selector),
                Aggregator.Minimum => x => x.Min(selector),
                Aggregator.Maximum => x => x.Max(selector),
                Aggregator.Diff => x => x.Diff(selector),
                Aggregator.Stddev => x => x.Stddev(selector),
                Aggregator.Count => x => x.Count(),
                Aggregator.Range => x => x.Max(selector) - x.Min(selector),
                Aggregator.Multiply => x => x.Multiply(selector),
                Aggregator.Last => x => x.Select(selector).Last(),
                _ => throw new NotImplementedException($"Aggregation method {aggregator} not yet implemented")
            };

            resampler.SampleAggregate(series, x => resultCast(aggregationFunc(x) * correctionFactor + correctionOffset));
        }

        private static double Median<TSource>(this IEnumerable<TSource> values, Func<TSource, double> selector)
        {
            var vals = values.Select(selector).ToList();
            return vals.OrderBy(x => x).Skip(Math.Max(0, Math.Min(vals.Count / 2, vals.Count - 1))).First();
        }

        private static double Diff<TSource>(this IEnumerable<TSource> values, Func<TSource, double> selector)
        {
            bool first = true;
            double result = 0;
            foreach (var val in values.Select(selector))
                result = first ? val : result - val;
            return result;
        }

        private static double Multiply<TSource>(this IEnumerable<TSource> values, Func<TSource, double> selector)
        {
            bool first = true;
            double result = 0;
            foreach (var val in values.Select(selector))
                result = first ? val : result * val;
            return result;
        }

        private static double Stddev<TSource>(this IEnumerable<TSource> values, Func<TSource, double> selector)
        {
            var vals = values.Select(selector).ToList();
            var sum = vals.Sum();
            var len = vals.Count;
            var avg = sum / len;
            sum = 0;
            foreach (var val in vals)
                sum += (val - avg) * (val - avg);
            return Math.Sqrt((sum / len));
        }
    }
}