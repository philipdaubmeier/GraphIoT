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
    public class DigitalstromEnergyViewModel : GraphCollectionViewModelBase
    {
        private readonly IDigitalstromDbContext db;
        private IQueryable<DigitalstromEnergyMidresData> dataMid = null;
        private IQueryable<DigitalstromEnergyHighresData> dataHigh = null;

        private readonly Dictionary<Dsuid, int> indices;

        private enum Resolution
        {
            MidRes,
            HighRes
        }
        private Resolution DataResolution => Span.Duration.TotalSeconds >= 30 ? Resolution.MidRes : Resolution.HighRes;

        private readonly IDigitalstromStructureService dsStructure;

        public DigitalstromEnergyViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService digitalstromStructure) : base()
        {
            db = databaseContext;
            dsStructure = digitalstromStructure;
            indices = Enumerable.Range(0, dsStructure.Circuits.Count()).Zip(dsStructure.Circuits.Where(x => dsStructure.IsMeteringCircuit(x)).OrderBy(x => x), (i, c) => new Tuple<Dsuid, int>(c, i)).ToDictionary(x => x.Item1, x => x.Item2);
            InvalidateData();
        }

        public override string Key => "energy";

        protected override void InvalidateData()
        {
            _energyGraphs = new Dictionary<Dsuid, GraphViewModel>();
            _loadedEnergyGraphs.Clear();
            dataMid = db?.DsEnergyMidresDataSet.Where(x => x.Day >= Span.Begin.Date && x.Day <= Span.End.Date);
            dataHigh = db?.DsEnergyHighresDataSet.Where(x => x.Day >= Span.Begin.Date && x.Day <= Span.End.Date);
        }
        
        public bool IsEmpty => (EnergyGraphs?.Count() ?? 0) <= 0 || !EnergyGraphs.First().Value.Points.Any();

        public override int GraphCount()
        {
            return indices.Count;
        }

        public override GraphViewModel Graph(int index)
        {
            LazyLoadEnergyGraphs(index);
            return _energyGraphs.Where(x => x.Key == indices.Where(i => i.Value == index).FirstOrDefault().Key).Select(x => x.Value).FirstOrDefault() ?? new GraphViewModel();
        }

        public override IEnumerable<GraphViewModel> Graphs()
        {
            foreach (var graph in EnergyGraphs.OrderBy(x => x.Key))
                yield return graph.Value;
        }
        
        private Dictionary<Dsuid, GraphViewModel> _energyGraphs = new Dictionary<Dsuid, GraphViewModel>();
        private readonly HashSet<int> _loadedEnergyGraphs = new HashSet<int>();
        public Dictionary<Dsuid, GraphViewModel> EnergyGraphs
        {
            get
            {
                LazyLoadEnergyGraphs();
                return _energyGraphs ?? new Dictionary<Dsuid, GraphViewModel>();
            }
        }

        private void LazyLoadEnergyGraphs(int index = -1)
        {
            // the requested index is already loaded
            if (index >= 0 && _loadedEnergyGraphs.Contains(index))
                return;

            // everything already loaded
            if (_loadedEnergyGraphs.Count >= indices.Count())
                return;

            var resamplers = IsInitialSpan ? indices.Keys.ToDictionary(x => x, x => new TimeSeriesResampler<TimeSeriesStream<int>, int>(Span))
                : DataResolution == Resolution.MidRes ? LazyLoadMidResEnergyGraphs(index) : LazyLoadHighResEnergyGraphs();

            if (IsInitialSpan)
                _loadedEnergyGraphs.UnionWith(indices.Values);

            foreach (var resampler in resamplers ?? new Dictionary<Dsuid, TimeSeriesResampler<TimeSeriesStream<int>, int>>())
                _energyGraphs.Add(resampler.Key, new GraphViewModel<int>(resampler.Value.Resampled, $"Stromverbrauch {dsStructure?.GetCircuitName(resampler.Key) ?? resampler.Key.ToString()}", $"stromverbrauch_{resampler.Key.ToString()}", "# Ws"));
        }

        private Dictionary<Dsuid, TimeSeriesResampler<TimeSeriesStream<int>, int>> LazyLoadHighResEnergyGraphs()
        {
            // already loaded or nothing to load
            if (_energyGraphs.Count > 0 || dataHigh == null)
                return null;

            // in a first pass read all compressed base64 encoded streams and reduce them to a timeseries with the final resolution.
            // with n days and m circuits we have m * n reduced time series after the first pass, but we did that trade because we
            // only need to load and decompress all database values once and we do not have to store all decompressed data in RAM.
            var reduced = indices.Keys.ToDictionary(x => x, x => new List<ITimeSeries<int>>());
            foreach (var dbEntry in dataHigh.ToList())
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
            var resamplers = indices.Keys.ToDictionary(x => x, x => new TimeSeriesResampler<TimeSeriesStream<int>, int>(Span, SamplingConstraint.NoOversampling));
            foreach (var resampler in resamplers)
                Aggregate(resampler.Value, reduced.Where(x => x.Key == resampler.Key).SelectMany(x => x.Value), Aggregator.Average, x => x, x => (int)x);
            _loadedEnergyGraphs.UnionWith(resamplers.SelectMany(x => indices.Where(i => i.Key == x.Key).Select(i => i.Value)));
            return resamplers;
        }

        private Dictionary<Dsuid, TimeSeriesResampler<TimeSeriesStream<int>, int>> LazyLoadMidResEnergyGraphs(int index = -1)
        {
            var filterDsuid = (string)indices.FirstOrDefault(x => x.Value == index).Key ?? string.Empty;
            var query = index < 0 ? dataMid : dataMid.Where(x => x.CircuitId == filterDsuid);
            var resamplers = new Dictionary<Dsuid, TimeSeriesResampler<TimeSeriesStream<int>, int>>();
            foreach (var series in dataMid.ToList().GroupBy(x => (Dsuid)x.CircuitId))
            {
                var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(Span, SamplingConstraint.NoOversampling);
                Aggregate(resampler, series.Select(x => x.EnergySeries), Aggregator.Average, x => x, x => (int)x);
                resamplers.Add(series.Key, resampler);
            }

            if (index < 0)
                _loadedEnergyGraphs.UnionWith(indices.Values);
            else
                _loadedEnergyGraphs.Add(index);

            return resamplers;
        }
    }
}