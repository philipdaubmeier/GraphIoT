using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.SmarthomeApi.Database.ViewModel
{
    public class DigitalstromZoneSensorViewModel : IGraphCollectionViewModel
    {
        private readonly PersistenceContext db;
        private readonly IQueryable<DigitalstromZoneSensorData> data;

        private readonly TimeSeriesSpan span;

        private readonly Dictionary<int, string> zoneNames;
        private readonly int zoneCount;

        public DigitalstromZoneSensorViewModel(PersistenceContext databaseContext, TimeSeriesSpan span)
        {
            db = databaseContext;
            this.span = span;
            data = db.DsSensorDataSet.Where(x => x.Day >= span.Begin.Date && x.Day <= span.End.Date);

            var zones = db.DsZones.ToList();
            zoneCount = zones.Count;
            zoneNames = zones.Where(x => x.Name != 0).ToDictionary(x => x.Id, x => x.Name.ToString());
        }

        public bool IsEmpty => (TemperatureGraphs?.Count() ?? 0) <= 0 || !TemperatureGraphs.First().Value.Points.Any();

        public int GraphCount()
        {
            return zoneCount * 2;
        }

        public GraphViewModel Graph(int index)
        {
            int i = index / 2;
            if (index % 2 == 0)
            {
                if (_temperatureGraphs == null || !_temperatureGraphs.Any(x => x.Key.Item1 == i))
                    LazyLoadTemperatureGraphs(i);
                return _temperatureGraphs.Where(x => x.Key.Item1 == i).FirstOrDefault().Value ?? new GraphViewModel();
            }
            else
            {
                if (_humidityGraphs == null || !_humidityGraphs.Any(x => x.Key.Item1 == i))
                    LazyLoadHumidityGraphs(i);
                return _humidityGraphs.Where(x => x.Key.Item1 == i).FirstOrDefault().Value ?? new GraphViewModel();
            }
        }

        public IEnumerable<GraphViewModel> Graphs()
        {
            foreach (var graph in TemperatureGraphs)
            {
                yield return graph.Value;
                if (HumidityGraphs.ContainsKey(graph.Key))
                    yield return HumidityGraphs[graph.Key];
            }
        }
        
        private Dictionary<Tuple<int, int>, GraphViewModel> _temperatureGraphs = null;
        public Dictionary<int, GraphViewModel> TemperatureGraphs
        {
            get
            {
                LazyLoadTemperatureGraphs();
                return _temperatureGraphs.ToDictionary(x => x.Key.Item2, x => x.Value) ?? new Dictionary<int, GraphViewModel>();
            }
        }

        private void LazyLoadTemperatureGraphs(int index = -1)
        {
            if ((_temperatureGraphs != null && _temperatureGraphs.Count >= zoneCount) || data == null)
                return;

            _temperatureGraphs = new Dictionary<Tuple<int, int>, GraphViewModel>();

            int i = -1;
            foreach (var series in data.ToList().GroupBy(x => x.ZoneId))
            {
                i++;
                if (index != -1 && index != i)
                    continue;

                var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
                resampler.SampleAverage(series.Select(x => x.TemperatureSeries), x => (decimal)x, x => (double)x);
                _temperatureGraphs.Add(new Tuple<int, int>(i, series.Key), new GraphViewModel<double>(resampler.Resampled, $"Temperatur {zoneNames.GetValueOrDefault(series.Key, "Zone " + series.Key)}", "#.# °C"));
            }
        }

        private Dictionary<Tuple<int, int>, GraphViewModel> _humidityGraphs = null;
        public Dictionary<int, GraphViewModel> HumidityGraphs
        {
            get
            {
                LazyLoadHumidityGraphs();
                return _humidityGraphs.ToDictionary(x => x.Key.Item2, x => x.Value) ?? new Dictionary<int, GraphViewModel>();
            }
        }

        private void LazyLoadHumidityGraphs(int index = -1)
        {
            if ((_humidityGraphs != null && _humidityGraphs.Count >= zoneCount) || data == null)
                return;

            _humidityGraphs = new Dictionary<Tuple<int, int>, GraphViewModel>();

            int i = -1;
            foreach (var series in data.ToList().GroupBy(x => x.ZoneId))
            {
                i++;
                if (index != -1 && index != i)
                    continue;

                var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
                resampler.SampleAverage(series.Select(x => x.HumiditySeries), x => (decimal)x, x => (double)x);
                _humidityGraphs.Add(new Tuple<int, int>(i, series.Key), new GraphViewModel<double>(resampler.Resampled, $"Luftfeuchtigkeit {zoneNames.GetValueOrDefault(series.Key, "Zone " + series.Key)}", "#.0 '%'"));
            }
        }
    }
}