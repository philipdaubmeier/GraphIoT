using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SonnenHost.Database;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using System.Linq;

namespace PhilipDaubmeier.SonnenHost.ViewModel
{
    public class SonnenEnergyViewModel : GraphCollectionViewModelDeferredLoadBase<SonnenEnergyData>
    {
        public SonnenEnergyViewModel(ISonnenDbContext databaseContext)
            : base(databaseContext?.SonnenEnergyDataSet, Enumerable.Range(0, 8).ToDictionary(x => x.ToString(), x => x))
        { }

        public override string Key => "solarenergy";

        public override GraphViewModel Graph(int index)
        {
            switch (index)
            {
                case 0: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Erzeugung", null, "# W");
                case 1: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Verbrauch", null, "# W");
                case 2: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Eigenverbrauch", null, "# W");
                case 3: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Batterieladung", null, "# W");
                case 4: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Batterieentladung", null, "# W");
                case 5: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Netzeinspeisung", null, "# W");
                case 6: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Netzbezug", null, "# W");
                case 7: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Batterie Ladestand", null, "#.# %");
                default: return new GraphViewModel();
            }
        }
    }
}