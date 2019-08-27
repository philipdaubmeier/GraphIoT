using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.DigitalstromHost.Structure;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromHost.ViewModel
{
    public class DigitalstromEnergyViewModel : GraphCollectionViewModelBase
    {
        private readonly IDigitalstromDbContext db;
        private IQueryable<DigitalstromEnergyHighresData> data = null;

        private readonly IDigitalstromStructureService dsStructure;

        public DigitalstromEnergyViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService digitalstromStructure) : base()
        {
            db = databaseContext;
            dsStructure = digitalstromStructure;
            InvalidateData();
        }

        public override string Key => "energy";

        protected override void InvalidateData()
        {
            _energyGraphs = null;
            data = db?.DsEnergyHighresDataSet.Where(x => x.Day >= Span.Begin.Date && x.Day <= Span.End.Date);
        }
        
        public bool IsEmpty => (EnergyGraphs?.Count() ?? 0) <= 0 || !EnergyGraphs.First().Value.Points.Any();

        public override int GraphCount()
        {
            return EnergyGraphs.Count;
        }

        public override GraphViewModel Graph(int index)
        {
            return Graphs().Skip(index).FirstOrDefault() ?? new GraphViewModel();
        }

        public override IEnumerable<GraphViewModel> Graphs()
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
                            resamplers.Add(series.Key, new TimeSeriesResampler<TimeSeriesStream<int>, int>(Span, SamplingConstraint.NoOversampling));

                        Aggregate(resamplers[series.Key], series.Value, Aggregator.Average, x => x, x => (int)x);
                    }
                }
            }

            foreach (var resampler in resamplers)
                _energyGraphs.Add(resampler.Key, new GraphViewModel<int>(resampler.Value.Resampled, $"Stromverbrauch {dsStructure?.GetCircuitName(resampler.Key) ?? resampler.Key.ToString()}", "# Ws"));
        }
    }
}