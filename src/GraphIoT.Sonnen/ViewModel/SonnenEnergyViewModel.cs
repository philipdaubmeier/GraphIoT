using Microsoft.Extensions.Localization;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Sonnen.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Sonnen.ViewModel
{
    public class SonnenEnergyViewModel : GraphCollectionViewModelMultiResolutionBase<SonnenEnergyData>
    {
        public SonnenEnergyViewModel(ISonnenDbContext databaseContext, IStringLocalizer<SonnenEnergyViewModel> localizer)
            : base(new Dictionary<Resolution, IQueryable<SonnenEnergyData>>() {
                       { Resolution.LowRes, databaseContext.SonnenEnergyLowresDataSet },
                       { Resolution.MidRes, databaseContext.SonnenEnergyDataSet }
                   },
                   Enumerable.Range(0, 8).ToDictionary(x => x.ToString(), x => x),
                   localizer)
        { }

        public override string Key => "solarenergy";

        public override GraphViewModel Graph(int index)
        {
            return index switch
            {
                0 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Production"], null, "# W"),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Consumption"], null, "# W"),
                2 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Direct usage"], null, "# W"),
                3 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Battery charging"], null, "# W"),
                4 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Battery discharging"], null, "# W"),
                5 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Grid feedin"], null, "# W"),
                6 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Grid purchase"], null, "# W"),
                7 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Battery SoC"], null, "#.# %"),
                _ => new GraphViewModel(),
            };
        }
    }
}