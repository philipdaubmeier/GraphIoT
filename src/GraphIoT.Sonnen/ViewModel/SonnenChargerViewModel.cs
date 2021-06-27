using Microsoft.Extensions.Localization;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Sonnen.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Sonnen.ViewModel
{
    public class SonnenChargerViewModel : GraphCollectionViewModelMultiResolutionBase<SonnenChargerData>
    {
        public SonnenChargerViewModel(ISonnenDbContext databaseContext, IStringLocalizer<SonnenChargerViewModel> localizer)
            : base(new Dictionary<Resolution, IQueryable<SonnenChargerData>>() {
                       { Resolution.LowRes, databaseContext.SonnenChargerLowresDataSet },
                       { Resolution.MidRes, databaseContext.SonnenChargerDataSet }
                   },
                   Enumerable.Range(0, 6).ToDictionary(x => x.ToString(), x => x),
                   localizer)
        { }

        public override string Key => "evcharger";

        public override GraphViewModel Graph(int index)
        {
            return index switch
            {
                0 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Charged energy"), null, "# Wh"),
                1 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Active power"), null, "# w"),
                2 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Current"), null, "#"),
                3 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Connected"), null, Localized("# (connected/disconnected)")),
                4 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Charging"), null, Localized("# (charging/not charging)")),
                5 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Smart mode"), null, Localized("# (smart mode/power mode)")),
                _ => new GraphViewModel(),
            };
        }
    }
}