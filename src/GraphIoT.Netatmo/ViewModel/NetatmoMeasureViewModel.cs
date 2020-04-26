using Microsoft.Extensions.Localization;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Structure;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Netatmo.ViewModel
{
    public class NetatmoMeasureViewModel : GraphCollectionViewModelKeyedItemBase<NetatmoMeasureData, Guid>
    {
        private readonly INetatmoDeviceService _netatmoStructure;

        public NetatmoMeasureViewModel(INetatmoDbContext databaseContext, INetatmoDeviceService netatmoStructure, IStringLocalizer<NetatmoMeasureViewModel> localizer)
            : base(new Dictionary<Resolution, IQueryable<NetatmoMeasureData>>() {
                       { Resolution.LowRes, databaseContext.NetatmoMeasureLowresDataSet },
                       { Resolution.MidRes, databaseContext.NetatmoMeasureDataSet }
                   },
                   Enumerable.Range(0, 1).ToDictionary(x => x.ToString(), x => x),
                   netatmoStructure.Modules.SelectMany(x => netatmoStructure.GetModuleMeasures(x.Item2)
                                               .Select(m => netatmoStructure.GetModuleMeasureDbId(x.Item2, m))
                                           )
                                           .Where(x => x != null && x.HasValue).Select(x => x!.Value)
                                           .OrderBy(x => netatmoStructure.GetDeviceId(x))
                                           .ThenBy(x => netatmoStructure.GetModuleId(x))
                                           .ThenBy(x => netatmoStructure.GetMeasure(x))
                                           .ToList(),
                   x => x.ModuleMeasureId,
                   key => x => x.ModuleMeasureId == key,
                   localizer)
        {
            _netatmoStructure = netatmoStructure;
        }

        public override string Key => "netatmo";

        public override GraphViewModel Graph(int index)
        {
            return DeferredLoadGraph<TimeSeries<double>, double>(index,
                k => $"{(MeasureType)(_netatmoStructure.GetMeasure(k) ?? MeasureType.Temperature)} {_netatmoStructure.GetModuleName(_netatmoStructure.GetModuleId(k) ?? string.Empty)} {_netatmoStructure.GetDeviceName(_netatmoStructure.GetDeviceId(k) ?? string.Empty)}",
                k => _localizer["measure_{0}_{1}", _netatmoStructure.GetModuleId(k), _netatmoStructure.GetMeasure(k)], "#.#");
        }
    }
}