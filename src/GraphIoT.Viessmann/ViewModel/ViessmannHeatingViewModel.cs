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
                0 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Burner minutes"), null, Localized("# min")),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Burner starts"), null, Localized("# starts")),
                2 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Burner modulation"), null, "# (1-255)"),
                3 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Outside temperature"), null, "#.# °C", RemoveZeros),
                4 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Boiler temperature"), null, "#.# °C", RemoveZeros),
                5 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Boiler temperature main"), null, "#.# °C", RemoveZeros),
                6 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Circuit 1 temperature"), null, "#.# °C", RemoveZeros),
                7 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Circuit 2 temperature"), null, "#.# °C", RemoveZeros),
                8 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Domestic hot water temperature"), null, "#.# °C", RemoveZeros),
                9 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Circuit 1 pump"), null, Localized("# (off/on)")),
                10 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Circuit 1 pump"), null, Localized("# (off/on)")),
                11 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Circuit 2 pump"), null, Localized("# (off/on)")),
                12 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Domestic hot water primary pump"), null, Localized("# (off/on)")),
                13 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Domestic hot water circulation pump"), null, Localized("# (off/on)")),
                _ => new GraphViewModel(),
            };
        }
    }
}