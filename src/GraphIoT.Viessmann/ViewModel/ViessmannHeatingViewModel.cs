using Microsoft.Extensions.Localization;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Viessmann.ViewModel
{
    public class ViessmannHeatingViewModel : GraphCollectionViewModelMultiResolutionBase<ViessmannHeatingData>
    {
        public ViessmannHeatingViewModel(IViessmannDbContext databaseContext, IStringLocalizer<ViessmannHeatingViewModel> localizer)
            : base(new Dictionary<Resolution, IQueryable<ViessmannHeatingData>>() {
                       { Resolution.LowRes, databaseContext.ViessmannHeatingLowresTimeseries },
                       { Resolution.MidRes, databaseContext.ViessmannHeatingTimeseries }
                   },
                   Enumerable.Range(0, 14).ToDictionary(x => x.ToString(), x => x),
                   localizer)
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
                0 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Burner minutes"], null, _localizer["# min"]),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Burner starts"], null, _localizer["# starts"]),
                2 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Burner modulation"], null, "# (1-255)"),
                3 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Outside temperature"], null, "#.# °C", RemoveZeros),
                4 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Boiler temperature"], null, "#.# °C", RemoveZeros),
                5 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Boiler temperature main"], null, "#.# °C", RemoveZeros),
                6 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Circuit 1 temperature"], null, "#.# °C", RemoveZeros),
                7 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Circuit 2 temperature"], null, "#.# °C", RemoveZeros),
                8 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Domestic hot water temperature"], null, "#.# °C", RemoveZeros),
                9 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Circuit 1 pump"], null, _localizer["# (off/on)"]),
                10 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Circuit 1 pump"], null, _localizer["# (off/on)"]),
                11 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Circuit 2 pump"], null, _localizer["# (off/on)"]),
                12 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Domestic hot water primary pump"], null, _localizer["# (off/on)"]),
                13 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Domestic hot water circulation pump"], null, _localizer["# (off/on)"]),
                _ => new GraphViewModel(),
            };
        }
    }
}