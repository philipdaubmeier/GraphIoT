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

            // in a first pass read all compressed base64 encoded streams and reduce them to a timeseries with the final resolution.
            // with n days and m circuits we have m * n reduced time series after the first pass, but we did that trade because we
            // only need to load and decompress all database values once and we do not have to store all decompressed data in RAM.
            var reduced = dsStructure.Circuits.ToDictionary(x => x, x => new List<ITimeSeries<int>>());
            foreach (var dbEntry in data.ToList())
            {
                using (var seriesCollection = dbEntry.EnergySeriesEveryMeter)
                {
                    foreach (var series in seriesCollection)
                    {
                        if (!reduced.ContainsKey(series.Key))
                            continue;

                        var reducedSpan = new TimeSeriesSpan(series.Value.Span.Begin, series.Value.Span.End, Span.Duration);
                        var reducer = new TimeSeriesResampler<TimeSeries<int>, int>(reducedSpan, SamplingConstraint.NoOversampling);

                        Aggregate(reducer, series.Value, Aggregator.Average, x => x, x => (int)x);

                        reduced[series.Key].Add(reducer.Resampled);
                    }
                }
            }

            // in the second pass we can then merge them into the final series - one time series object per circuit
            var resamplers = dsStructure.Circuits.ToDictionary(x => x, x => new TimeSeriesResampler<TimeSeriesStream<int>, int>(Span, SamplingConstraint.NoOversampling));
            foreach (var resampler in resamplers)
                Aggregate(resampler.Value, reduced.Where(x => x.Key == resampler.Key).SelectMany(x => x.Value), Aggregator.Average, x => x, x => (int)x);

            _energyGraphs = resamplers.ToDictionary(resampler => resampler.Key, resampler =>
                (GraphViewModel)new GraphViewModel<int>(resampler.Value.Resampled, $"Stromverbrauch {dsStructure?.GetCircuitName(resampler.Key) ?? resampler.Key.ToString()}", "# Ws"));
        }
    }
}