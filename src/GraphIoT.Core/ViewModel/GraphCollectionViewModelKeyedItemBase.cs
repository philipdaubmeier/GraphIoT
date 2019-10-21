using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    public abstract class GraphCollectionViewModelKeyedItemBase<Tentity, Tkey> : GraphCollectionViewModelMultiResolutionBase<Tentity> where Tentity : class, ITimeSeriesDbEntity where Tkey : notnull
    {
        protected readonly Dictionary<int, GraphViewModel> _loadedGraphs = new Dictionary<int, GraphViewModel>();
        protected readonly Dictionary<Tkey, int> _keys;
        protected readonly Func<Tentity, Tkey> _keySelector;
        protected readonly Func<Tkey, Func<Tentity, bool>> _predicateGenerator;

        protected GraphCollectionViewModelKeyedItemBase(Dictionary<Resolution, IQueryable<Tentity>> dataTables, Dictionary<string, int> columns, List<Tkey> keys, Func<Tentity, Tkey> keySelector, Func<Tkey, Func<Tentity, bool>> predicateGenerator)
            : base(dataTables, columns)
        {
            _keys = Enumerable.Range(0, keys.Count()).Zip(keys, (i, c) => new Tuple<Tkey, int>(c, i)).ToDictionary(x => x.Item1, x => x.Item2);
            _keySelector = keySelector;
            _predicateGenerator = predicateGenerator;
        }

        protected override void InvalidateData()
        {
            _loadedGraphs.Clear();
            base.InvalidateData();
        }

        public override int GraphCount() => _columns.Count * _keys.Count;

        protected GraphViewModel DeferredLoadGraph<Tseries, Tval>(int index, Func<Tkey, string> nameSelector, Func<Tkey, string> strKeySelector, string format, Func<ITimeSeries<Tval>, ITimeSeries<Tval>>? preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            // the requested index is already loaded
            if (_loadedGraphs.ContainsKey(index))
                return _loadedGraphs[index];

            int row = index / _columns.Count;
            int column = index % _columns.Count;

            // only an initial mock span is given, load nothing, build empty result
            if (IsInitialSpan)
            {
                if (!_loadedGraphs.ContainsKey(column))
                    foreach (var item in _keys.ToDictionary(x => column + x.Value * _columns.Count, x => BuildGraphViewModel<Tseries, Tval>(new List<TimeSeries<Tval>>(), nameSelector(x.Key), strKeySelector(x.Key), format)))
                        _loadedGraphs.Add(item.Key, item.Value);
            }
            else
            {
                var key = _keys.Where(x => x.Value == row).Select(x => x.Key).FirstOrDefault();
                var groupedLoadedData = data?.Where(_predicateGenerator(key))?.ToList()?.GroupBy(_keySelector);

                if (groupedLoadedData == null)
                    return new GraphViewModel();

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
            return new GraphViewModel();
        }
    }
}