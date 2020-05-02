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
                   Enumerable.Range(0, 10).ToDictionary(x => x.ToString(), x => x),
                   localizer)
        { }

        public override string Key => "weconnect";

        public override GraphViewModel Graph(int index)
        {
            return index switch
            {
                0 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Driven kilometers"], null, _localizer["# km"]),
                1 => DeferredLoadGraph<TimeSeries<int>, int>(index, _localizer["Battery SoC"], null, _localizer["# %"]),
                2 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Consumed kWh"], null, _localizer["#.# kWh"]),
                3 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Average consumption kWh/100 km"], null, _localizer["#.# kWh/100 km"]),
                4 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Average speed km/h"], null, _localizer["#.# km/h"]),
                5 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Charging state"], null, _localizer["# (charging/not charging)"]),
                6 => DeferredLoadGraph<TimeSeries<double>, double>(index, _localizer["Climate temperature"], null, _localizer["#.# °C"]),
                7 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Climate state"], null, _localizer["# (on/off)"]),
                8 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Window melt state"], null, _localizer["# (on/off)"]),
                9 => DeferredLoadGraph<TimeSeries<bool>, bool>(index, _localizer["Remote heating state"], null, _localizer["# (on/off)"]),
                _ => new GraphViewModel()
            };
        }
    }
}