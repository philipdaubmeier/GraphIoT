using Microsoft.Extensions.Logging;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoHost.Polling
{
    public class NetatmoWeatherPollingService : INetatmoPollingService
    {
        private readonly ILogger _logger;
        private readonly NetatmoWebClient _netatmoClient;

        public NetatmoWeatherPollingService(ILogger<NetatmoWeatherPollingService> logger, NetatmoWebClient netatmoClient)
        {
            _logger = logger;
            _netatmoClient = netatmoClient;
        }

        public async Task PollValues()
        {
            _logger.LogInformation($"{DateTime.Now} Netatmo Background Service is polling new weather station values...");

            try
            {
                await PollSensorValues(DateTime.Now.AddHours(-1), DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Netatmo weather background worker: {ex.Message}");
            }
        }

        public async Task PollSensorValues(DateTime start, DateTime end)
        {
            // TODO: polling implementation

            var homedata = await _netatmoClient.GetHomeData();

            var stationdata = await _netatmoClient.GetWeatherStationData();

            var station = stationdata.Devices.Where(s => s.StationName == "Phils Netatmo").FirstOrDefault();
            var deviceId = station.Id;
            var moduleId = station.Modules.Where(m => m.DataType.Contains(MeasureType.Temperature) &&
                                                      m.DataType.Contains(MeasureType.CO2)).FirstOrDefault().Id;

            var measures = await _netatmoClient.GetMeasure(deviceId, moduleId, new Measure[] { MeasureType.Temperature, MeasureType.CO2 });
        }
    }
}