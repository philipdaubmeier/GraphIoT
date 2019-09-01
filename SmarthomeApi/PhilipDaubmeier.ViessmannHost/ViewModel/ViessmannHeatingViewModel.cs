using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using PhilipDaubmeier.ViessmannHost.Database;
using System.Linq;

namespace PhilipDaubmeier.ViessmannHost.ViewModel
{
    public class ViessmannHeatingViewModel : GraphCollectionViewModelDeferredLoadBase<ViessmannHeatingData>
    {
        public ViessmannHeatingViewModel(IViessmannDbContext databaseContext)
            : base(databaseContext?.ViessmannHeatingTimeseries, Enumerable.Range(0, 14).ToDictionary(x => x.ToString(), x => x))
        { }

        public override string Key => "heating";

        public override GraphViewModel Graph(int index)
        {
            // Hack for noisy data which contains 0 values where actually no current data was there, should be null really
            TimeSeries<double> RemoveZeros(TimeSeries<double> input)
            {
                for (int i = 0; i < input.Count; i++)
                    if (input[i].HasValue && input[i].Value == 0d)
                        input[i] = null;
                return input;
            }

            switch (index)
            {
                case 0: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Betriebsstunden Brenner", null, "# min");
                case 1: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Brennerstarts", null, "# Starts");
                case 2: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Brennermodulation", null, "# (1-255)");
                case 3: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Außentemperatur", null, "#.# °C", RemoveZeros);
                case 4: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Kesselwassertemperatur", null, "#.# °C", RemoveZeros);
                case 5: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Kesseltemperatur", null, "#.# °C", RemoveZeros);
                case 6: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Vorlauftemperatur Heizkreis 1", null, "#.# °C", RemoveZeros);
                case 7: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Vorlauftemperatur Heizkreis 2", null, "#.# °C", RemoveZeros);
                case 8: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Warmwassertemperatur", null, "#.# °C", RemoveZeros);
                case 9: return DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Brenner aktiv", null, "# (aus/ein)");
                case 10: return DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Heizkreispumpe Heizkreis 1", null, "# (aus/ein)");
                case 11: return DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Heizkreispumpe Heizkreis 2", null, "# (aus/ein)");
                case 12: return DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Speicherladepumpe Warmwasser", null, "# (aus/ein)");
                case 13: return DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Zirkulationspumpe Warmwasser", null, "# (aus/ein)");
                default: return new GraphViewModel();
            }
        }
    }
}