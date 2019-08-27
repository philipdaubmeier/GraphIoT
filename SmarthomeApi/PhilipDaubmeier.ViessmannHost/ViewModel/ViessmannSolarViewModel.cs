using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using PhilipDaubmeier.ViessmannHost.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannHost.ViewModel
{
    public class ViessmannSolarViewModel : GraphCollectionViewModelBase
    {
        private readonly IViessmannDbContext db;
        private IQueryable<ViessmannSolarData> data;

        public ViessmannSolarViewModel(IViessmannDbContext databaseContext) : base()
        {
            db = databaseContext;
            InvalidateData();
        }

        public override string Key => "solar";

        protected override void InvalidateData()
        {
            _solarWh = null;
            _solarCollectorTemp = null;
            _solarHotwaterTemp = null;
            _solarPumpState = null;
            _solarSuppression = null;
            data = db?.ViessmannSolarTimeseries.Where(x => x.Day >= Span.Begin.Date && x.Day <= Span.End.Date);
        }

        public bool IsEmpty => !SolarWh.Points.Any();

        public override int GraphCount() => 5;

        public override GraphViewModel Graph(int index)
        {
            switch (index)
            {
                case 0: return SolarWh;
                case 1: return SolarCollectorTemp;
                case 2: return SolarHotwaterTemp;
                case 3: return SolarPumpState;
                case 4: return SolarSuppression;
                default: return new GraphViewModel();
            }
        }

        public override IEnumerable<GraphViewModel> Graphs()
        {
            return Enumerable.Range(0, GraphCount()).Select(i => Graph(i));
        }

        private GraphViewModel _solarWh = null;
        public GraphViewModel SolarWh
        {
            get
            {
                LazyLoadSolarWh();
                return _solarWh ?? new GraphViewModel();
            }
        }

        private void LazyLoadSolarWh()
        {
            if (_solarWh != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(Span, SamplingConstraint.NoOversampling);

            // Hack: remove first 5 elements due to bug in day-boundaries
            var preprocessedSeries = new List<TimeSeries<int>>();
            foreach (var series in data)
            {
                var whseries = series.SolarWhSeries;
                for (int i = 0; i < 5; i++)
                    whseries[i] = whseries[i].HasValue ? (int?)0 : null;
                preprocessedSeries.Add(whseries);
            }

            Aggregate(resampler, preprocessedSeries, Aggregator.Sum, x => x, x => (int)x);
            _solarWh = new GraphViewModel<int>(resampler.Resampled, "Produktion Wh", "# Wh");
        }

        private GraphViewModel _solarCollectorTemp = null;
        public GraphViewModel SolarCollectorTemp
        {
            get
            {
                LazyLoadSolarCollectorTemp();
                return _solarCollectorTemp ?? new GraphViewModel();
            }
        }

        private void LazyLoadSolarCollectorTemp()
        {
            if (_solarCollectorTemp != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(Span, SamplingConstraint.NoOversampling);
            Aggregate(resampler, data.Select(x => x.SolarCollectorTempSeries), Aggregator.Average, x => (decimal)x, x => (double)x);
            _solarCollectorTemp = new GraphViewModel<double>(resampler.Resampled, "Kollektortemperatur °C", "#.# °C");
        }

        private GraphViewModel _solarHotwaterTemp = null;
        public GraphViewModel SolarHotwaterTemp
        {
            get
            {
                LazyLoadSolarHotwaterTemp();
                return _solarHotwaterTemp ?? new GraphViewModel();
            }
        }

        private void LazyLoadSolarHotwaterTemp()
        {
            if (_solarHotwaterTemp != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(Span, SamplingConstraint.NoOversampling);
            Aggregate(resampler, data.Select(x => x.SolarHotwaterTempSeries), Aggregator.Average, x => (decimal)x, x => (double)x);
            _solarHotwaterTemp = new GraphViewModel<double>(resampler.Resampled, "Warmwassertemperatur °C", "#.# °C");
        }

        private GraphViewModel _solarPumpState = null;
        public GraphViewModel SolarPumpState
        {
            get
            {
                LazyLoadSolarPumpState();
                return _solarPumpState ?? new GraphViewModel();
            }
        }

        private void LazyLoadSolarPumpState()
        {
            if (_solarPumpState != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(Span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.SolarPumpStateSeries), x => x.Any(b => b));
            _solarPumpState = new GraphViewModel<bool>(resampler.Resampled, "Solarpumpe", "# (aus/ein)");
        }

        private GraphViewModel _solarSuppression = null;
        public GraphViewModel SolarSuppression
        {
            get
            {
                LazyLoadSolarSuppression();
                return _solarSuppression ?? new GraphViewModel();
            }
        }

        private void LazyLoadSolarSuppression()
        {
            if (_solarSuppression != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(Span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.SolarPumpStateSeries), x => x.Any(b => b));
            _solarSuppression = new GraphViewModel<bool>(resampler.Resampled, "Nachheizunterdrückung", "# (aus/ein)");
        }
    }
}