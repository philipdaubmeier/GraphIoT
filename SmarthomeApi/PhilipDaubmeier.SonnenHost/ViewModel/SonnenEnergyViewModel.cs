using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SonnenHost.Database;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.SonnenHost.ViewModel
{
    public class SonnenEnergyViewModel : IGraphCollectionViewModel
    {
        private readonly ISonnenDbContext db;
        private readonly IQueryable<SonnenEnergyData> data;

        private readonly TimeSeriesSpan span;

        public SonnenEnergyViewModel(ISonnenDbContext databaseContext, TimeSeriesSpan span)
        {
            db = databaseContext;
            this.span = span;
            data = db.SonnenEnergyDataSet.Where(x => x.Day >= span.Begin.Date && x.Day <= span.End.Date);
        }

        public bool IsEmpty => !BatteryUsoc.Points.Any();

        public int GraphCount() => 8;

        public GraphViewModel Graph(int index)
        {
            switch(index)
            {
                case 0: return ProductionPower;
                case 1: return ConsumptionPower;
                case 2: return DirectUsagePower;
                case 3: return BatteryCharging;
                case 4: return BatteryDischarging;
                case 5: return GridFeedin;
                case 6: return GridPurchase;
                case 7: return BatteryUsoc;
                default: return new GraphViewModel();
            }
        }

        public IEnumerable<GraphViewModel> Graphs()
        {
            return Enumerable.Range(0, GraphCount()).Select(i => Graph(i));
        }

        private GraphViewModel _productionPower = null;
        public GraphViewModel ProductionPower
        {
            get
            {
                LazyLoadProductionPower();
                return _productionPower ?? new GraphViewModel();
            }
        }

        private void LazyLoadProductionPower()
        {
            if (_productionPower != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.ProductionPowerSeries), x => (decimal)x, x => (int)x);
            _productionPower = new GraphViewModel<int>(resampler.Resampled, "Erzeugung", "# W");
        }

        private GraphViewModel _consumptionPower = null;
        public GraphViewModel ConsumptionPower
        {
            get
            {
                LazyLoadConsumptionPower();
                return _consumptionPower ?? new GraphViewModel();
            }
        }

        private void LazyLoadConsumptionPower()
        {
            if (_consumptionPower != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.ConsumptionPowerSeries), x => (decimal)x, x => (int)x);
            _consumptionPower = new GraphViewModel<int>(resampler.Resampled, "Verbrauch", "# W");
        }

        private GraphViewModel _directUsagePower = null;
        public GraphViewModel DirectUsagePower
        {
            get
            {
                LazyLoadDirectUsagePower();
                return _directUsagePower ?? new GraphViewModel();
            }
        }

        private void LazyLoadDirectUsagePower()
        {
            if (_directUsagePower != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.DirectUsagePowerSeries), x => (decimal)x, x => (int)x);
            _directUsagePower = new GraphViewModel<int>(resampler.Resampled, "Eigenverbrauch", "# W");
        }

        private GraphViewModel _batteryCharging = null;
        public GraphViewModel BatteryCharging
        {
            get
            {
                LazyLoadBatteryCharging();
                return _batteryCharging ?? new GraphViewModel();
            }
        }

        private void LazyLoadBatteryCharging()
        {
            if (_batteryCharging != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.BatteryChargingSeries), x => (decimal)x, x => (int)x);
            _batteryCharging = new GraphViewModel<int>(resampler.Resampled, "Batterieladung", "# W");
        }

        private GraphViewModel _batteryDischarging = null;
        public GraphViewModel BatteryDischarging
        {
            get
            {
                LazyLoadBatteryDischarging();
                return _batteryDischarging ?? new GraphViewModel();
            }
        }

        private void LazyLoadBatteryDischarging()
        {
            if (_batteryDischarging != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.BatteryDischargingSeries), x => (decimal)x, x => (int)x);
            _batteryDischarging = new GraphViewModel<int>(resampler.Resampled, "Batterieentladung", "# W");
        }

        private GraphViewModel _gridFeedin = null;
        public GraphViewModel GridFeedin
        {
            get
            {
                LazyLoadGridFeedin();
                return _gridFeedin ?? new GraphViewModel();
            }
        }

        private void LazyLoadGridFeedin()
        {
            if (_gridFeedin != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.GridFeedinSeries), x => (decimal)x, x => (int)x);
            _gridFeedin = new GraphViewModel<int>(resampler.Resampled, "Netzeinspeisung", "# W");
        }

        private GraphViewModel _gridPurchase = null;
        public GraphViewModel GridPurchase
        {
            get
            {
                LazyLoadGridPurchase();
                return _gridPurchase ?? new GraphViewModel();
            }
        }

        private void LazyLoadGridPurchase()
        {
            if (_gridPurchase != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.GridPurchaseSeries), x => (decimal)x, x => (int)x);
            _gridPurchase = new GraphViewModel<int>(resampler.Resampled, "Netzbezug", "# W");
        }

        private GraphViewModel _batteryUsoc = null;
        public GraphViewModel BatteryUsoc
        {
            get
            {
                LazyLoadBatteryUsoc();
                return _batteryUsoc ?? new GraphViewModel();
            }
        }

        private void LazyLoadBatteryUsoc()
        {
            if (_batteryUsoc != null || data == null)
                return;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(span, SamplingConstraint.NoOversampling);
            resampler.SampleAverage(data.Select(x => x.BatteryUsocSeries), x => (decimal)x, x => (double)x);
            _batteryUsoc = new GraphViewModel<double>(resampler.Resampled, "Batterie Ladestand", "#.# %");
        }
    }
}