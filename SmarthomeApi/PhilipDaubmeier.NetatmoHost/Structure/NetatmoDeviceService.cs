using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using PhilipDaubmeier.NetatmoClient.Model.WeatherStation;
using PhilipDaubmeier.NetatmoHost.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PhilipDaubmeier.NetatmoHost.Structure
{
    public class NetatmoDeviceService : INetatmoDeviceService
    {
        private Dictionary<Tuple<ModuleId, ModuleId>, List<Measure>> _modules = null;

        private Dictionary<Tuple<ModuleId, Measure>, Guid> _moduleDbIds = null;

        private Dictionary<ModuleId, string> _deviceNames = null;

        private Dictionary<ModuleId, string> _moduleNames = null;

        private readonly IServiceProvider _services;

        private readonly ILogger _logger;

        private readonly Semaphore _loadSemaphore = new Semaphore(1, 1);

        public NetatmoDeviceService(IServiceProvider services, ILogger<NetatmoDeviceService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public IEnumerable<ModuleId> Devices
        {
            get
            {
                LazyLoad();
                return _modules.Select(x => x.Key.Item1).Distinct().OrderBy(x => x);
            }
        }

        public IEnumerable<Tuple<ModuleId, ModuleId>> Modules
        {
            get
            {
                LazyLoad();
                return _modules.Select(x => x.Key).Distinct().OrderBy(x => x.Item1).ThenBy(x => x.Item2);
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

        public ModuleId GetDeviceId(Guid dbId)
        {
            LazyLoad();

            var db = _modules.Where(x => x.Key.Item2 == GetModuleId(dbId));
            if (!db.Any())
                return null;
            return db.First().Key.Item1;
        }

        public ModuleId GetModuleId(Guid dbId)
        {
            LazyLoad();

            var db = _moduleDbIds?.Where(x => x.Value == dbId);
            if (dbId == null || !db.Any())
                return null;
            return db.First().Key.Item1;
        }

        public Measure GetMeasure(Guid dbId)
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

            if (!_deviceNames.TryGetValue(module, out string name))
                return string.Empty;
            return crop < 0 ? name : name.Substring(0, Math.Min(name.Length, crop));
        }

        public string GetModuleName(ModuleId module, int crop = -1)
        {
            LazyLoad();

            if (!_moduleNames.TryGetValue(module, out string name))
                return string.Empty;
            return crop < 0 ? name : name.Substring(0, Math.Min(name.Length, crop));
        }

        public void RefreshDbGuids()
        {
            LazyLoad();
        }

        private void LazyLoad()
        {
            try
            {
                _loadSemaphore.WaitOne();

                using (var scope = _services.CreateScope())
                {
                    var netatmoClient = scope.ServiceProvider.GetService<NetatmoWebClient>();
                    var dbContext = scope.ServiceProvider.GetService<INetatmoDbContext>();

                    if (_modules == null)
                        LoadStructure(netatmoClient);

                    if (_moduleDbIds == null || (_modules != null && _moduleDbIds.Count < _modules.Count))
                        LoadDbIds(dbContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Netatmo Device Service: {ex.Message}");
                throw;
            }
            finally { _loadSemaphore.Release(); }
        }

        private void LoadStructure(NetatmoWebClient netatmoClient)
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
        }

        private void LoadDbIds(INetatmoDbContext databaseContext)
        {
            var loaded = databaseContext.NetatmoModuleMeasures.ToList();
            _moduleDbIds = loaded.GroupBy(x => new Tuple<ModuleId, Measure>(x.ModuleId, x.Measure)).ToDictionary(x => x.Key, x => x.First().Id);
        }
    }
}