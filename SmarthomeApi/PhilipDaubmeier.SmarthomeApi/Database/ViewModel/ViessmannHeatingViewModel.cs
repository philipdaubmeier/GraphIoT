using PhilipDaubmeier.CompactTimeSeries;
using SmarthomeApi.Database.Model;
using System.Collections.Generic;
using System.Linq;

namespace SmarthomeApi.Database.ViewModel
{
    public class ViessmannHeatingViewModel : IGraphCollectionViewModel
    {
        private readonly PersistenceContext db;
        private readonly IQueryable<ViessmannHeatingData> data;

        private readonly TimeSeriesSpan span;

        public ViessmannHeatingViewModel(PersistenceContext databaseContext, TimeSeriesSpan span)
        {
            db = databaseContext;
            this.span = span;
            data = db.ViessmannHeatingTimeseries.Where(x => x.Day >= span.Begin.Date && x.Day <= span.End.Date);
        }

        public bool IsEmpty => !BurnerMinutes.Points.Any();

        public int GraphCount() => 14;

        public GraphViewModel Graph(int index)
        {
            switch(index)
            {
                case 0: return BurnerMinutes;
                case 1: return BurnerStarts;
                case 2: return BurnerModulation;
                case 3: return OutsideTemp;
                case 4: return BoilerTemp;
                case 5: return BoilerTempMain;
                case 6: return Circuit0Temp;
                case 7: return Circuit1Temp;
                case 8: return DhwTemp;
                case 9: return BurnerActive;
                case 10: return Circuit0Pump;
                case 11: return Circuit1Pump;
                case 12: return DhwPrimaryPump;
                case 13: return DhwCirculationPump;
                default: return new GraphViewModel();
            }
        }

        public IEnumerable<GraphViewModel> Graphs()
        {
            return Enumerable.Range(0, GraphCount()).Select(i => Graph(i));
        }

        private GraphViewModel _burnerMinutes = null;
        public GraphViewModel BurnerMinutes
        {
            get
            {
                LazyLoadBurnerMinutes();
                return _burnerMinutes ?? new GraphViewModel();
            }
        }

        private void LazyLoadBurnerMinutes()
        {
            if (_burnerMinutes != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAccumulate(data.Select(x => x.BurnerMinutesSeries));
            _burnerMinutes = new GraphViewModel<double>(resampler.Resampled, "Betriebsstunden Brenner", "# min");
        }

        private GraphViewModel _burnerStarts = null;
        public GraphViewModel BurnerStarts
        {
            get
            {
                LazyLoadBurnerStarts();
                return _burnerStarts ?? new GraphViewModel();
            }
        }

        private void LazyLoadBurnerStarts()
        {
            if (_burnerStarts != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAccumulate(data.Select(x => x.BurnerStartsSeries));
            _burnerStarts = new GraphViewModel<int>(resampler.Resampled, "Brennerstarts", "# Starts");
        }

        private GraphViewModel _burnerModulation = null;
        public GraphViewModel BurnerModulation
        {
            get
            {
                LazyLoadBurnerModulation();
                return _burnerModulation ?? new GraphViewModel();
            }
        }

        private void LazyLoadBurnerModulation()
        {
            if (_burnerModulation != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.BurnerModulationSeries), x => x, x => (int)x);
            _burnerModulation = new GraphViewModel<int>(resampler.Resampled, "Brennermodulation", "# (1-255)");
        }

        private GraphViewModel _outsideTemp = null;
        public GraphViewModel OutsideTemp
        {
            get
            {
                LazyLoadOutsideTemp();
                return _outsideTemp ?? new GraphViewModel();
            }
        }

        private void LazyLoadOutsideTemp()
        {
            if (_outsideTemp != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.OutsideTempSeries), x => (decimal)x, x => (double)x);
            _outsideTemp = new GraphViewModel<double>(resampler.Resampled, "Außentemperatur", "#.# °C");
        }

        private GraphViewModel _boilerTemp = null;
        public GraphViewModel BoilerTemp
        {
            get
            {
                LazyLoadBoilerTemp();
                return _boilerTemp ?? new GraphViewModel();
            }
        }

        private void LazyLoadBoilerTemp()
        {
            if (_boilerTemp != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.BoilerTempSeries), x => (decimal)x, x => (double)x);
            _boilerTemp = new GraphViewModel<double>(resampler.Resampled, "Kesselwassertemperatur", "#.# °C");
        }

        private GraphViewModel _boilerTempMain = null;
        public GraphViewModel BoilerTempMain
        {
            get
            {
                LazyLoadBoilerTempMain();
                return _boilerTempMain ?? new GraphViewModel();
            }
        }

        private void LazyLoadBoilerTempMain()
        {
            if (_boilerTempMain != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.BoilerTempMainSeries), x => (decimal)x, x => (double)x);
            _boilerTempMain = new GraphViewModel<double>(resampler.Resampled, "Kesseltemperatur", "#.# °C");
        }

        private GraphViewModel _circuit0Temp = null;
        public GraphViewModel Circuit0Temp
        {
            get
            {
                LazyLoadCircuit0Temp();
                return _circuit0Temp ?? new GraphViewModel();
            }
        }

        private void LazyLoadCircuit0Temp()
        {
            if (_circuit0Temp != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.Circuit0TempSeries), x => (decimal)x, x => (double)x);
            _circuit0Temp = new GraphViewModel<double>(resampler.Resampled, "Vorlauftemperatur Heizkreis 1", "#.# °C");
        }

        private GraphViewModel _circuit1Temp = null;
        public GraphViewModel Circuit1Temp
        {
            get
            {
                LazyLoadCircuit1Temp();
                return _circuit1Temp ?? new GraphViewModel();
            }
        }

        private void LazyLoadCircuit1Temp()
        {
            if (_circuit1Temp != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.Circuit1TempSeries), x => (decimal)x, x => (double)x);
            _circuit1Temp = new GraphViewModel<double>(resampler.Resampled, "Vorlauftemperatur Heizkreis 2", "#.# °C");
        }

        private GraphViewModel _dhwTemp = null;
        public GraphViewModel DhwTemp
        {
            get
            {
                LazyLoadDhwTemp();
                return _dhwTemp ?? new GraphViewModel();
            }
        }

        private void LazyLoadDhwTemp()
        {
            if (_dhwTemp != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.DhwTempSeries), x => (decimal)x, x => (double)x);
            _dhwTemp = new GraphViewModel<double>(resampler.Resampled, "Warmwassertemperatur", "#.# °C");
        }

        private GraphViewModel _burnerActive = null;
        public GraphViewModel BurnerActive
        {
            get
            {
                LazyLoadBurnerActive();
                return _burnerActive ?? new GraphViewModel();
            }
        }

        private void LazyLoadBurnerActive()
        {
            if (_burnerActive != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.BurnerActiveSeries), x => x.Any(b => b));
            _burnerActive = new GraphViewModel<bool>(resampler.Resampled, "Brenner aktiv", "# (aus/ein)");
        }

        private GraphViewModel _circuit0Pump = null;
        public GraphViewModel Circuit0Pump
        {
            get
            {
                LazyLoadCircuit0Pump();
                return _circuit0Pump ?? new GraphViewModel();
            }
        }

        private void LazyLoadCircuit0Pump()
        {
            if (_circuit0Pump != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.Circuit0PumpSeries), x => x.Any(b => b));
            _circuit0Pump = new GraphViewModel<bool>(resampler.Resampled, "Heizkreispumpe Heizkreis 1", "# (aus/ein)");
        }

        private GraphViewModel _circuit1Pump = null;
        public GraphViewModel Circuit1Pump
        {
            get
            {
                LazyLoadCircuit1Pump();
                return _circuit1Pump ?? new GraphViewModel();
            }
        }

        private void LazyLoadCircuit1Pump()
        {
            if (_circuit1Pump != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.Circuit1PumpSeries), x => x.Any(b => b));
            _circuit1Pump = new GraphViewModel<bool>(resampler.Resampled, "Heizkreispumpe Heizkreis 2", "# (aus/ein)");
        }

        private GraphViewModel _dhwPrimaryPump = null;
        public GraphViewModel DhwPrimaryPump
        {
            get
            {
                LazyLoadDhwPrimaryPump();
                return _dhwPrimaryPump ?? new GraphViewModel();
            }
        }

        private void LazyLoadDhwPrimaryPump()
        {
            if (_dhwPrimaryPump != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.DhwPrimaryPumpSeries), x => x.Any(b => b));
            _dhwPrimaryPump = new GraphViewModel<bool>(resampler.Resampled, "Speicherladepumpe Warmwasser", "# (aus/ein)");
        }

        private GraphViewModel _dhwCirculationPump = null;
        public GraphViewModel DhwCirculationPump
        {
            get
            {
                LazyLoadDhwCirculationPump();
                return _dhwCirculationPump ?? new GraphViewModel();
            }
        }

        private void LazyLoadDhwCirculationPump()
        {
            if (_dhwCirculationPump != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAggregate(data.Select(x => x.DhwCirculationPumpSeries), x => x.Any(b => b));
            _dhwCirculationPump = new GraphViewModel<bool>(resampler.Resampled, "Zirkulationspumpe Warmwasser", "# (aus/ein)");
        }
    }
}