using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Structure;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Netatmo.ViewModel
{
    public class NetatmoMeasureViewModel : GraphCollectionViewModelKeyedItemBase<NetatmoMeasureData, Guid>
    {
        private readonly INetatmoDeviceService _netatmoStructure;

        public NetatmoMeasureViewModel(INetatmoDbContext databaseContext, INetatmoDeviceService netatmoStructure)
            : base(new Dictionary<Resolution, IQueryable<NetatmoMeasureData>>() {
                       { Resolution.LowRes, databaseContext?.NetatmoMeasureLowresDataSet },
                       { Resolution.MidRes, databaseContext?.NetatmoMeasureDataSet }
                   },
                   Enumerable.Range(0, 1).ToDictionary(x => x.ToString(), x => x),
                   netatmoStructure.Modules.SelectMany(x => netatmoStructure.GetModuleMeasures(x.Item2)
                                               .Select(m => netatmoStructure.GetModuleMeasureDbId(x.Item2, m))
                                           )
                                           .Where(x => x.HasValue).Select(x => x.Value)
                                           .OrderBy(x => netatmoStructure.GetDeviceId(x))
                                           .ThenBy(x => netatmoStructure.GetModuleId(x))
                                           .ThenBy(x => netatmoStructure.GetMeasure(x))
                                           .ToList(),
                   x => x.ModuleMeasureId,
                   key => x => x.ModuleMeasureId == key)
        {
            _netatmoStructure = netatmoStructure;
        }

        public override string Key => "netatmo";

        public override GraphViewModel Graph(int index)
        {
            return DeferredLoadGraph<TimeSeries<double>, double>(index,
                k => $"{((MeasureType)_netatmoStructure.GetMeasure(k)).ToString()} {_netatmoStructure.GetModuleName(_netatmoStructure.GetModuleId(k))} {_netatmoStructure.GetDeviceName(_netatmoStructure.GetDeviceId(k))}",
                k => $"measure_{_netatmoStructure.GetModuleId(k)}_{_netatmoStructure.GetMeasure(k)}", "#.#");
        }
    }
}