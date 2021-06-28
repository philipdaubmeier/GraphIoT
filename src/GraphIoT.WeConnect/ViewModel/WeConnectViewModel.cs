using Microsoft.Extensions.Localization;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.WeConnect.Database;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.WeConnect.ViewModel
{
    public class WeConnectViewModel : GraphCollectionViewModelMultiResolutionBase<WeConnectData>
    {
        public WeConnectViewModel(IWeConnectDbContext databaseContext, IStringLocalizer<WeConnectViewModel> localizer)
            : base(new Dictionary<Resolution, IQueryable<WeConnectData>>() {
                       { Resolution.LowRes, databaseContext.WeConnectLowresDataSet },
                       { Resolution.MidRes, databaseContext.WeConnectDataSet }
                   },
                   Enumerable.Range(0, 12).ToDictionary(x => x.ToString(), x => x),
                   localizer)
        { }

        public override string Key => "weconnect";

        public override GraphViewModel Graph(int index)
        {
            return index switch
            {
                0 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Driven kilometers"), null, Localized("# km")),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, Localized("Battery SoC"), null, Localized("# %")),
                2 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Trip length km"), null, Localized("# km")),
                3 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Trip duration min"), null, Localized("# min")),
                4 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Trip average speed km/h"), null, Localized("#.# km/h")),
                5 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Trip consumed kWh"), null, Localized("#.# kWh")),
                6 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Trip average consumption kWh/100 km"), null, Localized("#.# kWh/100 km")),
                7 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Charging state"), null, Localized("# (charging/not charging)")),
                8 => DeferredLoadGraph<TimeSeries<double>, double>(index, Localized("Climate temperature"), null, Localized("#.# °C")),
                9 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Climate state"), null, Localized("# (on/off)")),
                10 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Window melt state"), null, Localized("# (on/off)")),
                11 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, Localized("Remote heating state"), null, Localized("# (on/off)")),
                _ => new GraphViewModel()
            };
        }
    }
}