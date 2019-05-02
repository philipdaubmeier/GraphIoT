using Microsoft.Extensions.Logging;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.PropertyTree;
using PhilipDaubmeier.DigitalstromHost.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromHost.Polling
{
    public class DigitalstromSensorPollingService : IDigitalstromPollingService
    {
        private readonly ILogger _logger;
        private readonly IDigitalstromDbContext _dbContext;
        private readonly DigitalstromWebserviceClient _dsClient;

        public DigitalstromSensorPollingService(ILogger<DigitalstromSensorPollingService> logger, IDigitalstromDbContext databaseContext, DigitalstromWebserviceClient dsClient)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _dsClient = dsClient;
        }

        public async Task PollValues()
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is polling new sensor values...");
            
            try
            {
                await PollSensorValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Digitalstrom sensor background worker: {ex.Message}");
            }
        }

        private async Task PollSensorValues()
        {
            var sensorValues = (await _dsClient.GetZonesAndSensorValues()).Zones;
            
            foreach (var zone in sensorValues)
                if (zone != null && zone.Sensor != null)
                    SaveZoneSensorValues(zone.ZoneID, zone.Sensor.ToDictionary((Func<SensorTypeAndValues, Sensor>)(x => (Sensor)x.Type), x => x.Value));

            _dbContext.SaveChanges();
        }
        
        private void SaveZoneSensorValues(int zoneId, Dictionary<Sensor, double> sensorValues)
        {
            int temperatureType = 9;
            int humidityType = 13;

            var time = DateTime.Now;
            var day = time.Date;
            var dbSensorSeries = _dbContext.DsSensorDataSet.Where(x => x.ZoneId == zoneId && x.Day == day).FirstOrDefault();
            if (dbSensorSeries == null)
            {
                var dbZone = _dbContext.DsZones.Where(x => x.Id == zoneId).FirstOrDefault();
                if (dbZone == null)
                    _dbContext.DsZones.Add(dbZone = new DigitalstromZone() { Id = zoneId });
                
                _dbContext.DsSensorDataSet.Add(dbSensorSeries = new DigitalstromZoneSensorData() { ZoneId = zoneId, Zone = dbZone, Day = day });
            }

            if (sensorValues.ContainsKey(temperatureType))
            {
                var series = dbSensorSeries.TemperatureSeries;
                series[time] = sensorValues[temperatureType];
                dbSensorSeries.TemperatureSeries = series;
            }

            if (sensorValues.ContainsKey(humidityType))
            {
                var series = dbSensorSeries.HumiditySeries;
                series[time] = sensorValues[humidityType];
                dbSensorSeries.HumiditySeries = series;
            }
        }
    }
}