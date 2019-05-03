using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromHost.ViewModel
{
    public class DigitalstromEnergyViewModel : IGraphCollectionViewModel
    {
        private readonly IDigitalstromDbContext db;
        private readonly IQueryable<DigitalstromEnergyHighresData> data;

        private readonly TimeSeriesSpan span;

        private readonly Dictionary<Dsuid, string> circuitNames;

        public DigitalstromEnergyViewModel(IDigitalstromDbContext databaseContext, TimeSeriesSpan span)
        {
            db = databaseContext;
            this.span = span;
            data = db.DsEnergyHighresDataSet.Where(x => x.Day >= span.Begin.Date && x.Day <= span.End.Date);

            // TODO: store and load names of circuits
            circuitNames = new Dictionary<Dsuid, string>();
        }

        public bool IsEmpty => (EnergyGraphs?.Count() ?? 0) <= 0 || !EnergyGraphs.First().Value.Points.Any();

        public int GraphCount()
        {
            return EnergyGraphs.Count;
        }

        public GraphViewModel Graph(int index)
        {
            return Graphs().Skip(index).FirstOrDefault() ?? new GraphViewModel();
        }

        public IEnumerable<GraphViewModel> Graphs()
        {
            foreach (var graph in EnergyGraphs)
                yield return graph.Value;
        }
        
        private Dictionary<Dsuid, GraphViewModel> _energyGraphs = null;
        public Dictionary<Dsuid, GraphViewModel> EnergyGraphs
        {
            get
            {
                LazyLoadEnergyGraphs();
                return _energyGraphs ?? new Dictionary<Dsuid, GraphViewModel>();
            }
        }

        private void LazyLoadEnergyGraphs()
        {
            if (_energyGraphs != null || data == null)
                return;

            _energyGraphs = new Dictionary<Dsuid, GraphViewModel>();

            var resamplers = new Dictionary<Dsuid, TimeSeriesResampler<TimeSeriesStream<int>, int>>();
            foreach (var dbEntry in data.ToList())
            {
                using (var seriesCollection = dbEntry.EnergySeriesEveryMeter)
                {
                    foreach (var series in seriesCollection)
                    {
                        if (!resamplers.ContainsKey(series.Key))
                            resamplers.Add(series.Key, new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling));

                        resamplers[series.Key].SampleAccumulate(series.Value);
                    }
                }
            }

            foreach (var resampler in resamplers)
                _energyGraphs.Add(resampler.Key, new GraphViewModel<int>(resampler.Value.Resampled, $"Stromverbrauch {circuitNames.GetValueOrDefault(resampler.Key, resampler.Key.ToString())}", "# Ws"));
        }
    }
}