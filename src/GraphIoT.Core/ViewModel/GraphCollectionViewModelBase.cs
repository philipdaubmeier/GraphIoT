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
            switch (AggregatorFunction == Aggregator.Default ? defaultFunc : AggregatorFunction)
            {
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
                case Aggregator.Sum:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Sum(selector) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
                case Aggregator.Average: goto default;
                default:
                    {
                        resampler.SampleAggregate(series, x => resultCast(x.Average(selector) * CorrectionFactor + CorrectionOffset));
                        break;
                    }
            }
        }
    }
}