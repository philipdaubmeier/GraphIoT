using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.DigitalstromHost.Structure;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromHost.ViewModel
{
    public class DigitalstromZoneSensorViewModel : GraphCollectionViewModelBase
    {
        private readonly IDigitalstromDbContext db;
        private IQueryable<DigitalstromZoneSensorData> data;

        private readonly IDigitalstromStructureService dsStructure;
        private readonly int zoneCount;

        public DigitalstromZoneSensorViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService digitalstromStructure) : base()
        {
            db = databaseContext;
            dsStructure = digitalstromStructure;
            InvalidateData();

            zoneCount = dsStructure.Zones.Count();
        }

        public override string Key => "sensors";

        protected override void InvalidateData()
        {
            _temperatureGraphs = new Dictionary<Tuple<int, Zone>, GraphViewModel>();
            _loadedTemperatureGraphs.Clear();
            _humidityGraphs = new Dictionary<Tuple<int, Zone>, GraphViewModel>();
            _loadedHumidityGraphs.Clear();
            data = db?.DsSensorDataSet.Where(x => x.Day >= Span.Begin.Date && x.Day <= Span.End.Date);
        }

        public bool IsEmpty => (TemperatureGraphs?.Count() ?? 0) <= 0 || !TemperatureGraphs.First().Value.Points.Any();

        public override int GraphCount()
        {
            return zoneCount * 2;
        }

        public override GraphViewModel Graph(int index)
        {
            int i = index / 2;
            if (index % 2 == 0)
            {
                LazyLoadTemperatureGraphs(i);
                return _temperatureGraphs.Where(x => x.Key.Item1 == i).FirstOrDefault().Value ?? new GraphViewModel();
            }
            else
            {
                LazyLoadHumidityGraphs(i);
                return _humidityGraphs?.Where(x => x.Key.Item1 == i).FirstOrDefault().Value ?? new GraphViewModel();
            }
        }

        public override IEnumerable<GraphViewModel> Graphs()
        {
            foreach (var graph in TemperatureGraphs)
            {
                yield return graph.Value;
                if (HumidityGraphs.ContainsKey(graph.Key))
                    yield return HumidityGraphs[graph.Key];
            }
        }
        
        private Dictionary<Tuple<int, Zone>, GraphViewModel> _temperatureGraphs = new Dictionary<Tuple<int, Zone>, GraphViewModel>();
        private readonly HashSet<int> _loadedTemperatureGraphs = new HashSet<int>();
        public Dictionary<Zone, GraphViewModel> TemperatureGraphs
        {
            get
            {
                LazyLoadTemperatureGraphs();
                return _temperatureGraphs?.ToDictionary(x => x.Key.Item2, x => x.Value);
            }
        }

        private void LazyLoadTemperatureGraphs(int index = -1)
        {
            var resamplers = ResampleIfIndex(data?.ToList()?.GroupBy(x => (Zone)x.ZoneId), x => dsStructure.HasZoneSensor(x, SensorType.TemperatureIndoors), x => x.TemperatureSeries, _loadedTemperatureGraphs, index);
            foreach (var resampler in resamplers ?? new Dictionary<Tuple<int, Zone>, TimeSeriesResampler<TimeSeriesStream<double>, double>>())
                _temperatureGraphs.Add(resampler.Key, new GraphViewModel<double>(resampler.Value.Resampled, $"Temperatur {dsStructure?.GetZoneName(resampler.Key.Item2) ?? resampler.Key.Item2.ToString()}", $"temperatur_zone_{(int)resampler.Key.Item2}", "#.# °C"));
        }

        private Dictionary<Tuple<int, Zone>, GraphViewModel> _humidityGraphs = new Dictionary<Tuple<int, Zone>, GraphViewModel>();
        private readonly HashSet<int> _loadedHumidityGraphs = new HashSet<int>();
        public Dictionary<Zone, GraphViewModel> HumidityGraphs
        {
            get
            {
                LazyLoadHumidityGraphs();
                return _humidityGraphs.ToDictionary(x => x.Key.Item2, x => x.Value);
            }
        }

        private void LazyLoadHumidityGraphs(int index = -1)
        {
            var resamplers = ResampleIfIndex(data?.ToList()?.GroupBy(x => (Zone)x.ZoneId), x => dsStructure.HasZoneSensor(x, SensorType.HumidityIndoors), x => x.HumiditySeries, _loadedHumidityGraphs, index);
            foreach (var resampler in resamplers ?? new Dictionary<Tuple<int, Zone>, TimeSeriesResampler<TimeSeriesStream<double>, double>>())
                _humidityGraphs.Add(resampler.Key, new GraphViewModel<double>(resampler.Value.Resampled, $"Luftfeuchtigkeit {dsStructure?.GetZoneName(resampler.Key.Item2) ?? resampler.Key.Item2.ToString()}", $"luftfeuchtigkeit_zone_{(int)resampler.Key.Item2}", "#.0 '%'"));
        }

        private Dictionary<Tuple<int, Zone>, TimeSeriesResampler<TimeSeriesStream<double>, double>> ResampleIfIndex<T>(IEnumerable<IGrouping<Zone, T>> loadedData, Func<Zone, bool> zoneFilter, Func<T, TimeSeries<double>> seriesSelector, HashSet<int> loadedIndexSet, int index = -1)
        {
            var indices = Enumerable.Range(0, dsStructure.Zones.Count()).Zip(dsStructure.Zones.Where(zoneFilter).OrderBy(x => x), (i, z) => new Tuple<int, Zone>(i, z));

            // everything already loaded
            if (loadedIndexSet.Count >= indices.Count())
                return null;

            // the requested index is already loaded
            if (index >= 0 && loadedIndexSet.Contains(index))
                return null;

            // only an initial mock span is given, load nothing, build empty result
            if (IsInitialSpan)
            {
                loadedIndexSet.UnionWith(indices.Select(x => x.Item1));
                return indices.ToDictionary(x => x, x => new TimeSeriesResampler<TimeSeriesStream<double>, double>(Span));
            }

            var resamplers = new Dictionary<Tuple<int, Zone>, TimeSeriesResampler<TimeSeriesStream<double>, double>>();
            foreach (var series in loadedData)
            {
                var currentIndex = indices.Where(x => x.Item2 == series.Key).FirstOrDefault() ?? new Tuple<int, Zone>(-1, 0);
                if (currentIndex.Item1 < 0 || (index != -1 && currentIndex.Item1 != index))
                    continue;

                var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(Span, SamplingConstraint.NoOversampling);
                Aggregate(resampler, series.Select(seriesSelector), Aggregator.Average, x => (decimal)x, x => (double)x);
                resamplers.Add(currentIndex, resampler);
                loadedIndexSet.Add(currentIndex.Item1);
            }
            return resamplers;
        }
    }
}