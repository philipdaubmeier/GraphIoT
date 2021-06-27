using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using PhilipDaubmeier.NetatmoClient.Model.WeatherStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Structure
{
    public class NetatmoDeviceService : INetatmoDeviceService
    {
        private Dictionary<Tuple<ModuleId, ModuleId>, List<Measure>>? _modules = null;

        private Dictionary<Tuple<ModuleId, Measure>, Guid>? _moduleDbIds = null;

        private Dictionary<ModuleId, string>? _deviceNames = null;

        private Dictionary<ModuleId, string>? _moduleNames = null;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ILogger _logger;

        private readonly Semaphore _loadSemaphore = new Semaphore(1, 1);

        public NetatmoDeviceService(IServiceScopeFactory serviceScopeFactory, ILogger<NetatmoDeviceService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        private static readonly List<ModuleId> EmptyDeviceList = new List<ModuleId>();
        public IEnumerable<ModuleId> Devices
        {
            get
            {
                LazyLoad();
                return _modules?.Select(x => x.Key.Item1)?.Distinct()?.OrderBy(x => x) ?? EmptyDeviceList.OrderBy(x => x);
            }
        }

        private static readonly List<Tuple<ModuleId, ModuleId>> EmptyModuleList = new List<Tuple<ModuleId, ModuleId>>();
        public IEnumerable<Tuple<ModuleId, ModuleId>> Modules
        {
            get
            {
                LazyLoad();
                return _modules?.Select(x => x.Key)?.Distinct()?.OrderBy(x => x.Item1)?.ThenBy(x => x.Item2) ?? EmptyModuleList.OrderBy(x => x.Item1);
            }
        }

        public IEnumerable<Measure> GetModuleMeasures(ModuleId module)
        {
            LazyLoad();

            foreach (var measure in _modules.Where(x => x.Key.Item2 == module).FirstOrDefault().Value ?? new List<Measure>())
                yield return measure;
        }

        public Guid? GetModuleMeasureDbId(ModuleId module, Measure measure)
        {
            LazyLoad();

            var dbId = _moduleDbIds?.Where(x => x.Key.Item1 == module && x.Key.Item2 == measure);
            if (dbId == null || !dbId.Any())
                return null;
            return dbId.First().Value;
        }

        public ModuleId? GetDeviceId(Guid dbId)
        {
            LazyLoad();

            var moduleId = GetModuleId(dbId);
            if (moduleId is null)
                return null;

            var db = _modules.Where(x => x.Key.Item2 == moduleId);
            if (!db.Any())
                return null;
            return db.First().Key.Item1;
        }

        public ModuleId? GetModuleId(Guid dbId)
        {
            LazyLoad();

            var db = _moduleDbIds?.Where(x => x.Value == dbId);
            if (dbId == null || !db.Any())
                return null;
            return db.First().Key.Item1;
        }

        public Measure? GetMeasure(Guid dbId)
        {
            LazyLoad();

            var db = _moduleDbIds?.Where(x => x.Value == dbId);
            if (dbId == null || !db.Any())
                return null;
            return db.First().Key.Item2;
        }

        public string GetDeviceName(ModuleId module, int crop = -1)
        {
            LazyLoad();

            if (_deviceNames is null || !_deviceNames.TryGetValue(module, out string? name))
                return string.Empty;
            return (crop < 0 ? name : name?.Substring(0, Math.Min(name.Length, crop))) ?? string.Empty;
        }

        public string GetModuleName(ModuleId module, int crop = -1)
        {
            LazyLoad();

            if (_moduleNames is null || !_moduleNames.TryGetValue(module, out string? name))
                return string.Empty;
            return (crop < 0 ? name : name?.Substring(0, Math.Min(name.Length, crop))) ?? string.Empty;
        }

        public void ReloadFromNetatmoApi()
        {
            LazyLoad(true);
        }

        public void RefreshDbGuids()
        {
            LazyLoad();
        }

        private void LazyLoad(bool forceReloadFromNetatmoApi = false)
        {
            try
            {
                _loadSemaphore.WaitOne();

                using var scope = _serviceScopeFactory.CreateScope();
                var netatmoClient = scope.ServiceProvider.GetService<NetatmoWebClient>();
                var dbContext = scope.ServiceProvider.GetService<INetatmoDbContext>();

                if (netatmoClient is null)
                    throw new Exception("No NetatmoWebClient configured via dependency injection");
                if (dbContext is null)
                    throw new Exception("No INetatmoDbContext configured via dependency injection");

                if (_modules == null)
                    LoadStructureFromDb(dbContext);

                if (_modules == null || forceReloadFromNetatmoApi)
                    LoadStructureFromNetatmoApi(netatmoClient);

                if (_moduleDbIds == null || (_modules != null && _moduleDbIds.Count < _modules.Count))
                    LoadDbIds(dbContext);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Netatmo Device Service: {ex.Message}");
                throw;
            }
            finally { _loadSemaphore.Release(); }
        }

        private void LoadDbIds(INetatmoDbContext databaseContext)
        {
            var loaded = databaseContext.NetatmoModuleMeasures.ToList();
            _moduleDbIds = loaded.GroupBy(x => new Tuple<ModuleId, Measure>(x.ModuleId, x.Measure)).ToDictionary(x => x.Key, x => x.First().Id);
        }

        private void LoadStructureFromDb(INetatmoDbContext databaseContext)
        {
            var loaded = databaseContext.NetatmoModuleMeasures.ToList();
            if (loaded.Count == 0)
                return;

            _modules = loaded.GroupBy(x => new Tuple<ModuleId, ModuleId>(x.DeviceId, x.ModuleId))
                .ToDictionary(x => x.Key, x => x.Select(m => (Measure)m.Measure).ToList());

            _moduleNames = loaded.GroupBy(x => x.ModuleId)
                .ToDictionary(x => (ModuleId)x.Key, x => x.FirstOrDefault()?.ModuleName ?? string.Empty);

            _deviceNames = loaded.GroupBy(x => x.DeviceId)
                .ToDictionary(x => (ModuleId)x.Key, x => x.FirstOrDefault()?.StationName ?? string.Empty);
        }

        private void LoadStructureFromNetatmoApi(NetatmoWebClient netatmoClient)
        {
            var stationdata = netatmoClient.GetWeatherStationData().Result;

            _modules = stationdata.Devices
                .SelectMany(station => new[] { (ModuleBase)station }
                    .Union(station.Modules.Cast<ModuleBase>())
                    .ToDictionary(m => new Tuple<ModuleId, ModuleId>(station.Id, m.Id), m => m.DataType)
                )
                .ToDictionary(module => module.Key, module => module.Value);

            _moduleNames = stationdata.Devices
                .SelectMany(station => new[] { (ModuleBase)station }
                    .Union(station.Modules.Cast<ModuleBase>())
                    .ToDictionary(m => m.Id, m => m.ModuleName)
                ).ToDictionary(x => x.Key, x => x.Value);

            _deviceNames = stationdata.Devices.ToDictionary(x => x.Id, x => x.StationName);

            _moduleDbIds = null;
        }
    }
}