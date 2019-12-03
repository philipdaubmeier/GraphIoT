using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Viessmann.ViewModel
{
    public class ViessmannSolarViewModel : GraphCollectionViewModelMultiResolutionBase<ViessmannSolarData>
    {
        public ViessmannSolarViewModel(IViessmannDbContext databaseContext)
            : base(new Dictionary<Resolution, IQueryable<ViessmannSolarData>>() {
                       { Resolution.LowRes, databaseContext.ViessmannSolarLowresTimeseries },
                       { Resolution.MidRes, databaseContext.ViessmannSolarTimeseries }
                   },
                   Enumerable.Range(0, 5).ToDictionary(x => x.ToString(), x => x))
        { }

        public override string Key => "solar";

        public override GraphViewModel Graph(int index)
        {
            // Hack: remove first 5 elements due to bug in day-boundaries
            static ITimeSeries<int> PreprocessSolarProduction(ITimeSeries<int> input)
            {
                for (int i = 0; i < 5; i++)
                    input[i] = input[i].HasValue ? (int?)0 : null;
                return input;
            }

            return index switch
            {
                0 => DeferredLoadGraph<TimeSeries<int>, int>(index, "Produktion Wh", null, "# Wh", PreprocessSolarProduction),
                1 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Kollektortemperatur °C", null, "#.# °C"),
                2 => DeferredLoadGraph<TimeSeries<double>, double>(index, "Warmwassertemperatur °C", null, "#.# °C"),
                3 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Solarpumpe", null, "# (aus/ein)"),
                4 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Nachheizunterdrückung", null, "# (aus/ein)"),
                _ => new GraphViewModel(),
            };
        }
    }
}