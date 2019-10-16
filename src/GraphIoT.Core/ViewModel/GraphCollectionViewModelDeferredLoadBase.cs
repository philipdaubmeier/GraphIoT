using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    public abstract class GraphCollectionViewModelDeferredLoadBase<Tentity> : GraphCollectionViewModelBase where Tentity : class, ITimeSeriesDbEntity
    {
        protected IQueryable<Tentity> _dataTable;
        protected IQueryable<Tentity> data;

        protected readonly Dictionary<int, GraphViewModel> _graphs = new Dictionary<int, GraphViewModel>();
        protected readonly Dictionary<string, int> _columns;

        public GraphCollectionViewModelDeferredLoadBase(IQueryable<Tentity> dataTable, Dictionary<string, int> columns) : base()
        {
            _dataTable = dataTable;
            _columns = columns;
            InvalidateData();
        }

        protected override void InvalidateData()
        {
            _graphs.Clear();
            data = _dataTable?.Where(x => x.Key >= Span.Begin.Date && x.Key <= Span.End.Date);
        }

        public bool IsEmpty => !Graph(0).Points.Any();

        public override int GraphCount() => _columns.Count;

        public override IEnumerable<GraphViewModel> Graphs()
        {
            return Enumerable.Range(0, GraphCount()).Select(i => Graph(i));
        }

        protected GraphViewModel DeferredLoadGraph<Tseries, Tval>(int index, string name, string key, string format, Func<ITimeSeries<Tval>, ITimeSeries<Tval>> preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            // initial, i.e. empty graph view model
            if (IsInitialSpan)
                _graphs.Add(index, new GraphViewModel() { Name = name, Key = key ?? name, Format = format, Begin = Span.Begin, Spacing = Span.Duration, Points = new dynamic[] { } });

            // already loaded
            if (_graphs.ContainsKey(index))
                return _graphs[index];

            // nothing to load
            if (data == null)
                return new GraphViewModel();

            var series = data.Select(x => x.GetSeries<Tval>(index));
            _graphs.Add(index, BuildGraphViewModel<Tseries, Tval>(series, name, key, format, preprocess));
            return _graphs[index];
        }

        protected GraphViewModel BuildGraphViewModel<Tseries, Tval>(IEnumerable<ITimeSeries<Tval>> series, string name, string key, string format, Func<ITimeSeries<Tval>, ITimeSeries<Tval>> preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            return new GraphViewModel<Tval>(Resample<Tseries, Tval>(Span, series, preprocess), name, key ?? name, format);
        }

        protected Tseries Resample<Tseries, Tval>(TimeSeriesSpan span, IEnumerable<ITimeSeries<Tval>> series, Func<ITimeSeries<Tval>, ITimeSeries<Tval>> preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            var resampler = new TimeSeriesResampler<Tseries, Tval>(span, SamplingConstraint.NoOversampling);

            var dataToResample = series;
            if (preprocess != null)
                dataToResample = dataToResample.Select(x => preprocess(x));

            if (typeof(Tval) == typeof(int))
            {
                if (typeof(Tseries) == typeof(TimeSeries<int>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeries<int>, int>, dataToResample.Cast<ITimeSeries<int>>(), Aggregator.Average, x => x, x => (int)x);
                else if (typeof(Tseries) == typeof(TimeSeriesStream<int>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeriesStream<int>, int>, dataToResample.Cast<ITimeSeries<int>>(), Aggregator.Average, x => x, x => (int)x);
            }
            else if (typeof(Tval) == typeof(double))
            {
                if (typeof(Tseries) == typeof(TimeSeries<double>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeries<double>, double>, dataToResample.Cast<ITimeSeries<double>>(), Aggregator.Average, x => (decimal)x, x => (double)x);
                else if (typeof(Tseries) == typeof(TimeSeriesStream<double>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeriesStream<double>, double>, dataToResample.Cast<ITimeSeries<double>>(), Aggregator.Average, x => (decimal)x, x => (double)x);
            }
            else if (typeof(Tval) == typeof(bool))
                (resampler as TimeSeriesResampler<TimeSeries<bool>, bool>)?.SampleAggregate(dataToResample.Cast<ITimeSeries<bool>>(), x => x.Any(b => b));

            return resampler.Resampled;
        }
    }
}