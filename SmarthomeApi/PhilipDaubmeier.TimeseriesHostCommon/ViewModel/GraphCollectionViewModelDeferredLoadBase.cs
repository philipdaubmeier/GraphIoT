using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public abstract class GraphCollectionViewModelDeferredLoadBase<Tentity> : GraphCollectionViewModelBase where Tentity : class, ITimeSeriesDbEntity
    {
        private readonly DbSet<Tentity> _dataTable;
        private IQueryable<Tentity> data;

        private readonly Dictionary<int, GraphViewModel> _graphs = new Dictionary<int, GraphViewModel>();
        private readonly HashSet<int> _loadedGraphs = new HashSet<int>();
        private readonly Dictionary<string, int> _indices;

        public GraphCollectionViewModelDeferredLoadBase(DbSet<Tentity> dataTable, Dictionary<string, int> indices) : base()
        {
            _dataTable = dataTable;
            _indices = indices;
            InvalidateData();
        }

        protected override void InvalidateData()
        {
            _graphs.Clear();
            _loadedGraphs.Clear();
            data = _dataTable?.Where(x => x.Key >= Span.Begin.Date && x.Key <= Span.End.Date);
        }

        public bool IsEmpty => !Graph(0).Points.Any();

        public override int GraphCount() => _indices.Count;

        public override IEnumerable<GraphViewModel> Graphs()
        {
            return Enumerable.Range(0, GraphCount()).Select(i => Graph(i));
        }

        protected GraphViewModel DeferredLoadGraph<Tseries, Tval>(int index, string name, string key, string format, Func<TimeSeries<Tval>, TimeSeries<Tval>> preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            // nothing to load
            if (data == null)
                return new GraphViewModel(); ;

            // already loaded
            if (_loadedGraphs.Contains(index) && _graphs.ContainsKey(index))
                return _graphs[index];

            var resampler = new TimeSeriesResampler<Tseries, Tval>(Span, SamplingConstraint.NoOversampling);

            var dataToResample = data.Select(x => x.GetSeries<Tval>(index));
            if (preprocess != null)
                dataToResample = dataToResample.Select(x => preprocess(x));

            if (typeof(Tval) == typeof(int))
            {
                if (typeof(Tseries) == typeof(TimeSeries<int>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeries<int>, int>, dataToResample.Cast<TimeSeries<int>>(), Aggregator.Average, x => x, x => (int)x);
                else if (typeof(Tseries) == typeof(TimeSeriesStream<int>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeriesStream<int>, int>, dataToResample.Cast<TimeSeries<int>>(), Aggregator.Average, x => x, x => (int)x);
            }
            else if (typeof(Tval) == typeof(double))
            {
                if (typeof(Tseries) == typeof(TimeSeries<double>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeries<double>, double>, dataToResample.Cast<TimeSeries<double>>(), Aggregator.Average, x => (decimal)x, x => (double)x);
                else if (typeof(Tseries) == typeof(TimeSeriesStream<double>))
                    Aggregate(resampler as TimeSeriesResampler<TimeSeriesStream<double>, double>, dataToResample.Cast<TimeSeries<double>>(), Aggregator.Average, x => (decimal)x, x => (double)x);
            }
            else if (typeof(Tval) == typeof(bool))
                (resampler as TimeSeriesResampler<TimeSeries<bool>, bool>)?.SampleAggregate(dataToResample.Cast<TimeSeries<bool>>(), x => x.Any(b => b));

            _graphs.Add(index, new GraphViewModel<Tval>(resampler.Resampled, name, key ?? name, format));
            _loadedGraphs.Add(index);
            return _graphs[index];
        }
    }
}