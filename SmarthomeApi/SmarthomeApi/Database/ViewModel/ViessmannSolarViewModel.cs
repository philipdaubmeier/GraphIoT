using CompactTimeSeries;
using SmarthomeApi.Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace SmarthomeApi.Database.ViewModel
{
    public class ViessmannSolarViewModel : IGraphCollectionViewModel
    {
        private readonly PersistenceContext db;
        private readonly IQueryable<ViessmannSolarData> data;

        private readonly TimeSeriesSpan span;

        public ViessmannSolarViewModel(PersistenceContext databaseContext, TimeSeriesSpan span)
        {
            db = databaseContext;
            this.span = span;
            data = db.ViessmannSolarTimeseries.Where(x => x.Day >= span.Begin.Date && x.Day <= span.End.Date);
        }

        public bool IsEmpty => !SolarWh.Points.Any();

        public int GraphCount() => 5;

        public GraphViewModel Graph(int index)
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

        public IEnumerable<GraphViewModel> Graphs()
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

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            foreach (var series in data)
            {
                // Hack: remove first 5 elements due to bug in day-boundaries
                var whseries = series.SolarWhSeries;
                for (int i = 0; i < 5; i++)
                    whseries[i] = whseries[i].HasValue ? (int?)0 : null;

                resampler.SampleAccumulate(whseries);
            }
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

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.SolarCollectorTempSeries), x => (decimal)x, x => (double)x);
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

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.SolarHotwaterTempSeries), x => (decimal)x, x => (double)x);
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

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(span, SamplingConstraint.NoOversampling);
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

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.SolarPumpStateSeries), x => x.Any(b => b));
            _solarSuppression = new GraphViewModel<bool>(resampler.Resampled, "Nachheizunterdrückung", "# (aus/ein)");
        }
    }
}