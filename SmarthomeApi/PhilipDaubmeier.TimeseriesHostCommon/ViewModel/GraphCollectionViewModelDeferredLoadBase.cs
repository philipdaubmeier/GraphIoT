﻿using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public abstract class GraphCollectionViewModelDeferredLoadBase<Tentity> : GraphCollectionViewModelBase where Tentity : class, ITimeSeriesDbEntity
    {
        protected DbSet<Tentity> _dataTable;
        protected IQueryable<Tentity> data;

        protected readonly Dictionary<int, GraphViewModel> _graphs = new Dictionary<int, GraphViewModel>();
        protected readonly Dictionary<string, int> _columns;

        public GraphCollectionViewModelDeferredLoadBase(DbSet<Tentity> dataTable, Dictionary<string, int> columns) : base()
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

        protected GraphViewModel DeferredLoadGraph<Tseries, Tval>(int index, string name, string key, string format, Func<TimeSeries<Tval>, TimeSeries<Tval>> preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            // nothing to load
            if (data == null)
                return new GraphViewModel(); ;

            // already loaded
            if (_graphs.ContainsKey(index))
                return _graphs[index];

            var series = data.Select(x => x.GetSeries<Tval>(index));
            _graphs.Add(index, BuildGraphViewModel<Tseries, Tval>(series, name, key, format, preprocess));
            return _graphs[index];
        }

        protected GraphViewModel BuildGraphViewModel<Tseries, Tval>(IEnumerable<TimeSeries<Tval>> series, string name, string key, string format, Func<TimeSeries<Tval>, TimeSeries<Tval>> preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            var resampler = new TimeSeriesResampler<Tseries, Tval>(Span, SamplingConstraint.NoOversampling);

            var dataToResample = series;
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

            return new GraphViewModel<Tval>(resampler.Resampled, name, key ?? name, format);
        }
    }
}