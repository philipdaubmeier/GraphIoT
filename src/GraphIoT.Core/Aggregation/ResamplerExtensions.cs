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
            Func<IEnumerable<double>, double> aggregationFunc = GetAggregationFunction(aggregator);

            resampler.SampleAggregate(series, x => resultCast(aggregationFunc(x.Select(selector)) * correctionFactor + correctionOffset));

            if (aggregator == Aggregator.AverageZero)
            {
                var resamplingRate = Math.Max(1d, series.First().Span.Duration / resampler.Resampled.Span.Duration);
                resampler.SampleAggregate(series, x => resultCast(x.Sum(selector) / resamplingRate * correctionFactor + correctionOffset));
            }
        }

        public static double Aggregate(this IEnumerable<double> values, Aggregator aggregator)
        {
            return GetAggregationFunction(aggregator)(values);
        }

        private static Func<IEnumerable<double>, double> GetAggregationFunction(Aggregator aggregator)
        {
            return aggregator switch
            {
                var a when
                    a == Aggregator.Average ||
                    a == Aggregator.Default => x => x.Average(),
                Aggregator.AverageZero => x => x.Sum() - x.Count(),
                Aggregator.Median => x => x.Median(),
                Aggregator.Sum => x => x.Sum(),
                Aggregator.Minimum => x => x.Min(),
                Aggregator.Maximum => x => x.Max(),
                Aggregator.Diff => x => x.Diff(),
                Aggregator.Stddev => x => x.Stddev(),
                Aggregator.Count => x => x.Count(),
                Aggregator.Range => x => x.Max() - x.Min(),
                Aggregator.Multiply => x => x.Multiply(),
                Aggregator.Last => x => x.Last(),
                _ => throw new NotImplementedException($"Aggregation method {aggregator} not yet implemented")
            };
        }

        private static double Median(this IEnumerable<double> values)
        {
            var vals = values.ToList();
            return vals.OrderBy(x => x).Skip(Math.Max(0, Math.Min(vals.Count / 2, vals.Count - 1))).First();
        }

        private static double Diff(this IEnumerable<double> values)
        {
            bool first = true;
            double result = 0;
            foreach (var val in values)
                result = first ? val : result - val;
            return result;
        }

        private static double Multiply(this IEnumerable<double> values)
        {
            bool first = true;
            double result = 0;
            foreach (var val in values)
                result = first ? val : result * val;
            return result;
        }

        private static double Stddev(this IEnumerable<double> values)
        {
            var vals = values.ToList();
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