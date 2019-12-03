using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Viessmann.ViewModel
{
    public class ViessmannHeatingViewModel : GraphCollectionViewModelMultiResolutionBase<ViessmannHeatingData>
    {
        public ViessmannHeatingViewModel(IViessmannDbContext databaseContext)
            : base(new Dictionary<Resolution, IQueryable<ViessmannHeatingData>>() {
                       { Resolution.LowRes, databaseContext.ViessmannHeatingLowresTimeseries },
                       { Resolution.MidRes, databaseContext.ViessmannHeatingTimeseries }
                   },
                   Enumerable.Range(0, 14).ToDictionary(x => x.ToString(), x => x))
        { }

        public override string Key => "heating";

        public override GraphViewModel Graph(int index)
        {
            // Hack for noisy data which contains 0 values where actually no current data was there, should be null really
            static ITimeSeries<double> RemoveZeros(ITimeSeries<double> input)
            {
                for (int i = 0; i < input.Count; i++)
                    if (input[i].HasValue && input[i] == 0d)
                        input[i] = null;
                return input;
            }

            return index switch
            {
                0 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Betriebsstunden Brenner", null, "# min"),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, "Brennerstarts", null, "# Starts"),
                2 => DeferredLoadGraph<TimeSeries<int>, int>(index, "Brennermodulation", null, "# (1-255)"),
                3 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Außentemperatur", null, "#.# °C", RemoveZeros),
                4 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Kesselwassertemperatur", null, "#.# °C", RemoveZeros),
                5 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Kesseltemperatur", null, "#.# °C", RemoveZeros),
                6 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Vorlauftemperatur Heizkreis 1", null, "#.# °C", RemoveZeros),
                7 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Vorlauftemperatur Heizkreis 2", null, "#.# °C", RemoveZeros),
                8 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Warmwassertemperatur", null, "#.# °C", RemoveZeros),
                9 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Brenner aktiv", null, "# (aus/ein)"),
                10 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Heizkreispumpe Heizkreis 1", null, "# (aus/ein)"),
                11 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Heizkreispumpe Heizkreis 2", null, "# (aus/ein)"),
                12 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Speicherladepumpe Warmwasser", null, "# (aus/ein)"),
                13 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Zirkulationspumpe Warmwasser", null, "# (aus/ein)"),
                _ => new GraphViewModel(),
            };
        }
    }
}