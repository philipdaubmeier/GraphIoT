using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using PhilipDaubmeier.ViessmannHost.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannHost.ViewModel
{
    public class ViessmannSolarViewModel : GraphCollectionViewModelMultiResolutionBase<ViessmannSolarData>
    {
        public ViessmannSolarViewModel(IViessmannDbContext databaseContext)
            : base(new Dictionary<Resolution, IQueryable<ViessmannSolarData>>() {
                       { Resolution.LowRes, databaseContext?.ViessmannSolarLowresTimeseries },
                       { Resolution.MidRes, databaseContext?.ViessmannSolarTimeseries }
                   },
                   Enumerable.Range(0, 5).ToDictionary(x => x.ToString(), x => x))
        { }

        public override string Key => "solar";

        public override GraphViewModel Graph(int index)
        {
            // Hack: remove first 5 elements due to bug in day-boundaries
            ITimeSeries<int> PreprocessSolarProduction(ITimeSeries<int> input)
            {
                for (int i = 0; i < 5; i++)
                    input[i] = input[i].HasValue ? (int?)0 : null;
                return input;
            }

            switch (index)
            {
                case 0: return DeferredLoadGraph<TimeSeries<int>, int>(index, "Produktion Wh", null, "# Wh", PreprocessSolarProduction);
                case 1: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Kollektortemperatur °C", null, "#.# °C");
                case 2: return DeferredLoadGraph<TimeSeries<double>, double>(index, "Warmwassertemperatur °C", null, "#.# °C");
                case 3: return DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Solarpumpe", null, "# (aus/ein)");
                case 4: return DeferredLoadGraph<TimeSeries<bool>, bool>(index, "Nachheizunterdrückung", null, "# (aus/ein)");
                default: return new GraphViewModel();
            }
        }
    }
}