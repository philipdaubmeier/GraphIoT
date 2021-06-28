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
                0 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Production"), null, "# W"),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Consumption"), null, "# W"),
                2 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Direct usage"), null, "# W"),
                3 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Battery charging"), null, "# W"),
                4 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Battery discharging"), null, "# W"),
                5 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Grid feedin"), null, "# W"),
                6 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Grid purchase"), null, "# W"),
                7 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Battery SoC"), null, "#.# %"),
                _ => new GraphViewModel(),
            };
        }
    }
}