using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using System;
using System.Collections.Generic;

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

        private double correctionFactor = 1d;
        public double CorrectionFactor
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

        private double correctionOffset = 0d;
        public double CorrectionOffset
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
    }
}