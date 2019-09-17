using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoHost.Structure
{
    public interface INetatmoDeviceService
    {
        IEnumerable<ModuleId> Devices { get; }

        IEnumerable<Tuple<ModuleId, ModuleId>> Modules { get; }

        IEnumerable<Measure> GetModuleMeasures(ModuleId module);

        Guid? GetModuleMeasureDbId(ModuleId module, Measure measure);

        ModuleId GetDeviceId(Guid dbId);

        ModuleId GetModuleId(Guid dbId);

        Measure GetMeasure(Guid dbId);

        string GetDeviceName(ModuleId module, int crop = -1);

        string GetModuleName(ModuleId module, int crop = -1);

        void ReloadFromNetatmoApi();

        void RefreshDbGuids();
    }
}