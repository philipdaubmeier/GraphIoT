using CompactTimeSeries;
using SmarthomeApi.Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace SmarthomeApi.Database.ViewModel
{
    public class DigitalstromZoneSensorViewModel
    {
        private readonly PersistenceContext db;
        private readonly IQueryable<DigitalstromZoneSensorData> data;

        private readonly TimeSeriesSpan span;

        private readonly Dictionary<int, string> zoneNames;

        public DigitalstromZoneSensorViewModel(PersistenceContext databaseContext, TimeSeriesSpan span)
        {
            db = databaseContext;
            this.span = span;
            data = db.DsSensorDataSet.Where(x => x.Day >= span.Begin.Date && x.Day <= span.End.Date);

            zoneNames = db.DsZones.ToDictionary(x => x.Id, x => x.Name.ToString());
        }

        public bool IsEmpty => (TemperatureGraphs?.Count() ?? 0) <= 0 || !TemperatureGraphs.First().Value.Points.Any();

        public IEnumerable<GraphViewModel> AllGraphs()
        {
            foreach (var graph in TemperatureGraphs)
            {
                yield return graph.Value;
                if (HumidityGraphs.ContainsKey(graph.Key))
                    yield return HumidityGraphs[graph.Key];
            }
        }
        
        private Dictionary<int, GraphViewModel> _temperatureGraphs = null;
        public Dictionary<int, GraphViewModel> TemperatureGraphs
        {
            get
            {
                LazyLoadTemperatureGraphs();
                return _temperatureGraphs ?? new Dictionary<int, GraphViewModel>();
            }
        }

        private void LazyLoadTemperatureGraphs()
        {
            if (_temperatureGraphs != null || data == null)
                return;

            _temperatureGraphs = new Dictionary<int, GraphViewModel>();

            foreach (var series in data.ToList().GroupBy(x => x.ZoneId))
            {
                var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
                resampler.SampleAverage(series.Select(x => x.TemperatureSeries), x => (decimal)x, x => (double)x);
                _temperatureGraphs.Add(series.Key, new GraphViewModel<double>(resampler.Resampled, $"Temperatur {zoneNames.GetValueOrDefault(series.Key, "Zone " + series.Key)}", "#.# °C"));
            }
        }

        private Dictionary<int, GraphViewModel> _humidityGraphs = null;
        public Dictionary<int, GraphViewModel> HumidityGraphs
        {
            get
            {
                LazyLoadHumidityGraphs();
                return _humidityGraphs ?? new Dictionary<int, GraphViewModel>();
            }
        }

        private void LazyLoadHumidityGraphs()
        {
            if (_humidityGraphs != null || data == null)
                return;

            _humidityGraphs = new Dictionary<int, GraphViewModel>();

            foreach (var series in data.ToList().GroupBy(x => x.ZoneId))
            {
                var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
                resampler.SampleAverage(series.Select(x => x.TemperatureSeries), x => (decimal)x, x => (double)x);
                _humidityGraphs.Add(series.Key, new GraphViewModel<double>(resampler.Resampled, $"Luftfeuchtigkeit {zoneNames.GetValueOrDefault(series.Key, "Zone " + series.Key)}", "#.# °C"));
            }
        }
    }
}