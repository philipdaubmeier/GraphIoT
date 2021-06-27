using Microsoft.Extensions.Localization;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.ViewModel
{
    public class DigitalstromEnergyViewModel : GraphCollectionViewModelKeyedItemBase<DigitalstromEnergyData, Dsuid>
    {
        private readonly IDigitalstromDbContext _db;
        private readonly IDigitalstromStructureService _dsStructure;

        private IQueryable<DigitalstromEnergyHighresData>? dataHigh = null;

        public DigitalstromEnergyViewModel(IDigitalstromDbContext databaseContext, IDigitalstromStructureService dsStructure, IStringLocalizer<DigitalstromEnergyViewModel> localizer)
            : base(new Dictionary<Resolution, IQueryable<DigitalstromEnergyData>>() {
                       { Resolution.LowRes, databaseContext.DsEnergyLowresDataSet },
                       { Resolution.MidRes, databaseContext.DsEnergyMidresDataSet }
                   },
                   Enumerable.Range(0, 1).ToDictionary(x => x.ToString(), x => x),
                   dsStructure.Circuits.Where(x => dsStructure.IsMeteringCircuit(x)).OrderBy(x => x).ToList(),
                   x => x.CircuitId,
                   key => { string keystr = key; return x => x.CircuitId == keystr; },
                   localizer)
        {
            _db = databaseContext;
            _dsStructure = dsStructure;

            InvalidateData();
        }

        public override string Key => "energy";

        protected override void InvalidateData()
        {
            base.InvalidateData();

            if (_db is null)
                return;
            dataHigh = _db.DsEnergyHighresDataSet.Where(x => x.Key >= Span.Begin.Date && x.Key <= Span.End.Date);
        }

        public override GraphViewModel Graph(int index)
        {
            int column = index % _columns.Count;
            return column switch
            {
                0 => DeferredLoadGraph<TimeSeriesStream<int>, int>(index, k => Localized("Power consumption {0}", _dsStructure?.GetCircuitName(k) ?? k.ToString()), k => Localized("powerconsumption_{0}", k), "# Ws"),
                _ => new GraphViewModel(),
            };
        }

        protected new GraphViewModel DeferredLoadGraph<Tseries, Tval>(int index, Func<Dsuid, string> nameSelector, Func<Dsuid, string> strKeySelector, string format, Func<ITimeSeries<Tval>, ITimeSeries<Tval>>? preprocess = null) where Tval : struct where Tseries : TimeSeriesBase<Tval>
        {
            // initial, low and mid resolution loading is handled by base class
            if (DataResolution != Resolution.HighRes)
                return base.DeferredLoadGraph<Tseries, Tval>(index, nameSelector, strKeySelector, format, preprocess);

            // already loaded or nothing to load
            if (dataHigh == null || _loadedGraphs.Count > 0)
                return _loadedGraphs.ContainsKey(index) ? _loadedGraphs[index] : new GraphViewModel();

            // in a first pass read all compressed base64 encoded streams and reduce them to a timeseries with the final resolution.
            // with n days and m circuits we have m * n reduced time series after the first pass, but we did that trade because we
            // only need to load and decompress all database values once and we do not have to store all decompressed data in RAM.
            var reduced = _keys.Keys.ToDictionary(x => x, x => new List<ITimeSeries<Tval>>());
            foreach (var dbEntry in dataHigh.ToList())
            {
                using var seriesCollection = dbEntry.EnergySeriesEveryMeter;
                foreach (var series in seriesCollection.Where(s => reduced.ContainsKey(s.Key)))
                    reduced[series.Key].Add(Resample<Tseries, Tval>(
                        new TimeSeriesSpan(series.Value.Span.Begin, series.Value.Span.End, Span.Duration),
                        new List<ITimeSeries<Tval>>() { (series.Value as ITimeSeries<Tval>)! }) as ITimeSeries<Tval>);
            }

            // in the second pass we can then merge them into the final series - one time series object per circuit
            foreach (var circuit in _keys)
                _loadedGraphs.Add(circuit.Value, BuildGraphViewModel<Tseries, Tval>(reduced.Where(x => x.Key == circuit.Key).SelectMany(x => x.Value), nameSelector(circuit.Key), strKeySelector(circuit.Key), format));

            return _loadedGraphs.ContainsKey(index) ? _loadedGraphs[index] : new GraphViewModel();
        }
    }
}