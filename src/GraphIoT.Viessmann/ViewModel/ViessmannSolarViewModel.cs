using Microsoft.Extensions.Localization;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Viessmann.ViewModel
{
    public class ViessmannSolarViewModel : GraphCollectionViewModelMultiResolutionBase<ViessmannSolarData>
    {
        public ViessmannSolarViewModel(IViessmannDbContext databaseContext, IStringLocalizer<ViessmannSolarViewModel> localizer)
            : base(new Dictionary<Resolution, IQueryable<ViessmannSolarData>>() {
                       { Resolution.LowRes, databaseContext.ViessmannSolarLowresTimeseries },
                       { Resolution.MidRes, databaseContext.ViessmannSolarTimeseries }
                   },
                   Enumerable.Range(0, 5).ToDictionary(x => x.ToString(), x => x),
                   localizer)
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
                0 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Solar Wh"], null, "# Wh", PreprocessSolarProduction),
                1 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Solar collector temperature °C"], null, "#.# °C"),
                2 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Solar hot water temperature °C"], null, "#.# °C"),
                3 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Solar pump"], null, _localizer["# (off/on)"]),
                4 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Solar suppression"], null, _localizer["# (off/on)"]),
                _ => new GraphViewModel(),
            };
        }
    }
}