using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using SmarthomeApi.Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace SmarthomeApi.Database.ViewModel
{
    public class DigitalstromEnergyViewModel : IGraphCollectionViewModel
    {
        private readonly PersistenceContext db;
        private readonly IQueryable<DigitalstromEnergyHighresData> data;

        private readonly TimeSeriesSpan span;

        private readonly Dictionary<DSUID, string> circuitNames;

        public DigitalstromEnergyViewModel(PersistenceContext databaseContext, TimeSeriesSpan span)
        {
            db = databaseContext;
            this.span = span;
            data = db.DsEnergyHighresDataSet.Where(x => x.Day >= span.Begin.Date && x.Day <= span.End.Date);

            // TODO: store and load names of circuits
            circuitNames = new Dictionary<DSUID, string>();
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
        
        private Dictionary<DSUID, GraphViewModel> _energyGraphs = null;
        public Dictionary<DSUID, GraphViewModel> EnergyGraphs
        {
            get
            {
                LazyLoadEnergyGraphs();
                return _energyGraphs ?? new Dictionary<DSUID, GraphViewModel>();
            }
        }

        private void LazyLoadEnergyGraphs()
        {
            if (_energyGraphs != null || data == null)
                return;

            _energyGraphs = new Dictionary<DSUID, GraphViewModel>();

            var resamplers = new Dictionary<DSUID, TimeSeriesResampler<TimeSeriesStream<int>, int>>();
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