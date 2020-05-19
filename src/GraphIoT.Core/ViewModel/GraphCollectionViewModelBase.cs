using PhilipDaubmeier.CompactTimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    public abstract class GraphCollectionViewModelBase : IGraphCollectionViewModel
    {
        public abstract string Key { get; }

        public GraphCollectionViewModelBase()
        {
            span = new TimeSeriesSpan(DateTime.MinValue, DateTime.MinValue.AddMinutes(1), 1);
        }

        protected bool IsInitialSpan => Span.Begin == DateTime.MinValue && Span.End == DateTime.MinValue.AddMinutes(1) && Span.Count == 1;

        private TimeSeriesSpan span;
        public TimeSeriesSpan Span
        {
            get { return span; }
            set
            {
                if (value == null || (span != null && span.Begin == value.Begin && span.End == value.End && span.Count == value.Count))
                    return;
                span = value;

                InvalidateData();
            }
        }

        private Aggregator aggregatorFunction = Aggregator.Default;
        public Aggregator AggregatorFunction
        {
            get { return aggregatorFunction; }
            set
            {
                if (value == aggregatorFunction)
                    return;
                aggregatorFunction = value;

                InvalidateData();
            }
        }

        private decimal correctionFactor = 1M;
        public decimal CorrectionFactor
        {
            get { return correctionFactor; }
            set
            {
                if (value == correctionFactor)
                    return;
                correctionFactor = value;

                InvalidateData();
            }
        }

        private decimal correctionOffset = 0M;
        public decimal CorrectionOffset
        {
            get { return correctionOffset; }
            set
            {
                if (value == correctionOffset)
                    return;
                correctionOffset = value;

                InvalidateData();
            }
        }

        protected abstract void InvalidateData();

        public abstract int GraphCount();

        public abstract IEnumerable<string> GraphKeys();

        public abstract GraphViewModel Graph(int index);

        public abstract IEnumerable<GraphViewModel> Graphs();

        protected void Aggregate<Tseries, Tval>(TimeSeriesResampler<Tseries, Tval> resampler, ITimeSeries<Tval> series, Aggregator defaultFunc, Func<Tval, decimal> selector, Func<decimal, Tval> resultCast) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            Aggregate(resampler, new List<ITimeSeries<Tval>>() { series }, defaultFunc, selector, resultCast);
        }

        protected void Aggregate<Tseries, Tval>(TimeSeriesResampler<Tseries, Tval> resampler, IEnumerable<ITimeSeries<Tval>> series, Aggregator defaultFunc, Func<Tval, decimal> selector, Func<decimal, Tval> resultCast) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            static decimal Diff(IEnumerable<decimal> values)
            {
                bool first = true;
                decimal result = 0;
                foreach (var val in values)
                    result = first ? val : result - val;
                return result;
            }

            static decimal Multiply(IEnumerable<decimal> values)
            {
                bool first = true;
                decimal result = 0;
                foreach (var val in values)
                    result = first ? val : result * val;
                return result;
            }

            static decimal Stddev(IEnumerable<decimal> values)
            {
                var vals = values.ToList();
                var sum = vals.Sum();
                var len = vals.Count;
                var avg = sum / len;
                sum = 0;
                foreach (var val in vals)
                    sum += (val - avg) * (val - avg);
                return (decimal)Math.Sqrt((double)(sum / len));
            }

            var resamplingFactor = (decimal)Math.Max(1d, series.First().Span.Duration / resampler.Resampled.Span.Duration);
            switch (AggregatorFunction == Aggregator.Default ? defaultFunc : AggregatorFunction)
            {
                case Aggregator.Average:
                default:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Average(selector) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.AverageZero:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Sum(selector) / resamplingFactor * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Median:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Select(selector).OrderBy(x => x).Skip(Math.Max(0, Math.Min(x.Count() / 2, x.Count() - 1))).First() * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Sum:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Sum(selector) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Minimum:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Min(selector) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Maximum:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Max(selector) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Diff:
                    {
                        resampler.SampleAggregate(series, x => resultCast(Diff(x.Select(selector)) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Stddev:
                    {
                        resampler.SampleAggregate(series, x => resultCast(Stddev(x.Select(selector)) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Count:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Count() * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Range:
                    {
                        resampler.SampleAggregate(series, x => resultCast((x.Max(selector) - x.Min(selector)) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Multiply:
                    {
                        resampler.SampleAggregate(series, x => resultCast(Multiply(x.Select(selector)) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Last:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Select(selector).Last() * CorrectionFactor + CorrectionOffset));
                        break;
                    }
            }
        }
    }
}