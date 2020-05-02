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
                0 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Driven kilometers"], null, _localizer["# km"]),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Battery SoC"], null, _localizer["# %"]),
                2 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Trip length km"], null, _localizer["# km"]),
                3 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Trip duration min"], null, _localizer["# min"]),
                4 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Trip average speed km/h"], null, _localizer["#.# km/h"]),
                5 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Trip consumed kWh"], null, _localizer["#.# kWh"]),
                6 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Trip average consumption kWh/100 km"], null, _localizer["#.# kWh/100 km"]),
                7 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Charging state"], null, _localizer["# (charging/not charging)"]),
                8 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Climate temperature"], null, _localizer["#.# °C"]),
                9 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Climate state"], null, _localizer["# (on/off)"]),
                10 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Window melt state"], null, _localizer["# (on/off)"]),
                11 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Remote heating state"], null, _localizer["# (on/off)"]),
                _ => new GraphViewModel()
            };
        }
    }
}