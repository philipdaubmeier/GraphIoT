using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public abstract class GraphCollectionViewModelKeyedItemBase<Tentity, Tkey> : GraphCollectionViewModelMultiResolutionBase<Tentity> where Tentity : class, ITimeSeriesDbEntity
    {
        protected readonly Dictionary<int, GraphViewModel> _loadedGraphs = new Dictionary<int, GraphViewModel>();
        protected readonly Dictionary<Tkey, int> _keys;
        protected readonly Func<Tentity, Tkey> _keySelector;

        protected GraphCollectionViewModelKeyedItemBase(Dictionary<Resolution, DbSet<Tentity>> dataTables, Dictionary<string, int> columns, List<Tkey> keys, Func<Tentity, Tkey> keySelector)
            : base(dataTables, columns)
        {
            _keys = Enumerable.Range(0, keys.Count()).Zip(keys, (i, c) => new Tuple<Tkey, int>(c, i)).ToDictionary(x => x.Item1, x => x.Item2);
            _keySelector = keySelector;
        }

        protected override void InvalidateData()
        {
            _loadedGraphs.Clear();
            base.InvalidateData();
        }

        public override int GraphCount() => _columns.Count * _keys.Count;

        protected GraphViewModel DeferredLoadGraph<Tseries, Tval>(int index, Func<Tkey, string> nameSelector, Func<Tkey, string> strKeySelector, string format, Func<TimeSeries<Tval>, TimeSeries<Tval>> preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            // the requested index is already loaded
            if (_loadedGraphs.ContainsKey(index))
                return _loadedGraphs[index];

            int row = index / _columns.Count;
            int column = index % _columns.Count;

            var key = _keys.Where(x => x.Value == row).Select(x => x.Key).FirstOrDefault();
            var loadedData = data.Where(x => _keySelector(x).Equals(key)).ToList();

            var groupedLoadedData = loadedData.GroupBy(_keySelector);

            // only an initial mock span is given, load nothing, build empty result
            if (IsInitialSpan)
            {
                foreach (var item in _keys.ToDictionary(x => x.Value, x => BuildGraphViewModel<Tseries, Tval>(new List<TimeSeries<Tval>>(), nameSelector(x.Key), strKeySelector(x.Key), format)))
                    _loadedGraphs.Add(item.Key, item.Value);
            }
            else
            {
                var resamplers = new Dictionary<int, TimeSeriesResampler<TimeSeriesStream<double>, double>>();
                foreach (var data in groupedLoadedData)
                {
                    var currentRow = _keys.Where(x => x.Key.Equals(data.Key)).Any() ? _keys.Where(x => x.Key.Equals(data.Key)).FirstOrDefault().Value : -1;
                    if (currentRow < 0 || currentRow != row)
                        continue;

                    var timeseries = data.Select(x => x.GetSeries<Tval>(column));
                    var graph = BuildGraphViewModel<Tseries, Tval>(timeseries, nameSelector(data.Key), strKeySelector(data.Key), format, preprocess);
                    _loadedGraphs.Add(index, graph);
                }
            }

            if (_loadedGraphs.ContainsKey(index))
                return _loadedGraphs[index];
            return null;
        }
    }
}