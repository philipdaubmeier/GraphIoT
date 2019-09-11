using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.DigitalstromHost.Structure;
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
        private readonly DigitalstromDssClient _dsClient;
        private readonly IDigitalstromStructureService _dsStructure;

        public DigitalstromSensorPollingService(ILogger<DigitalstromSensorPollingService> logger, IDigitalstromDbContext databaseContext, DigitalstromDssClient dsClient, IDigitalstromStructureService dsStructure)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _dsClient = dsClient;
            _dsStructure = dsStructure;
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

            var timestamp = DateTime.UtcNow;
            var day = timestamp.Date;

            var readMidres = new Dictionary<Zone, Dictionary<Sensor, ITimeSeries<double>>>();
            foreach (var zone in sensorValues)
                if (zone != null && zone.Sensor != null)
                    readMidres.Add(zone.ZoneID, ReadAndSaveMidresZoneSensorValues(day, zone.ZoneID, zone.Sensor.ToDictionary(x => x.Type, x => x.Value), timestamp));

            SaveLowresZoneSensorValues(readMidres);

            _dbContext.SaveChanges();
        }

        public void GenerateLowResSensorSeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                var sensorZones = _dsStructure.Zones.Where(x => _dsStructure.HasZoneSensor(x, SensorType.TemperatureIndoors) || _dsStructure.HasZoneSensor(x, SensorType.HumidityIndoors));
                SaveLowresZoneSensorValues(sensorZones.ToDictionary(zone => zone, zone => ReadAndSaveMidresZoneSensorValues(day, zone)));

                _dbContext.SaveChanges();
            }
        }

        private Dictionary<Sensor, ITimeSeries<double>> ReadAndSaveMidresZoneSensorValues(DateTime day, int zoneId, Dictionary<Sensor, double> sensorValues = null, DateTime timestampNewValue = default)
        {
            var readOnly = sensorValues == null || timestampNewValue == default;
            var readSeries = new Dictionary<Sensor, ITimeSeries<double>>();

            var dbSensorSeries = GetOrCreateEntity(_dbContext.DsSensorDataSet, day, zoneId);

            void ReadAndSaveSensor(Sensor sensor, TimeSeries<double> series, int index)
            {
                if (!readOnly)
                {
                    if (!sensorValues.ContainsKey(sensor))
                        return;

                    series[timestampNewValue] = sensorValues[sensor];
                    dbSensorSeries.SetSeries(index, series);
                }
                readSeries.Add(sensor, series);
            }

            ReadAndSaveSensor(SensorType.TemperatureIndoors, dbSensorSeries.TemperatureSeries, 0);
            ReadAndSaveSensor(SensorType.HumidityIndoors, dbSensorSeries.HumiditySeries, 1);

            return readSeries;
        }

        private void SaveLowresZoneSensorValues(Dictionary<Zone, Dictionary<Sensor, ITimeSeries<double>>> midresSeries)
        {
            foreach (var zone in midresSeries)
            {
                DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
                var day = zone.Value.FirstOrDefault().Value?.Span?.Begin.Date ?? DateTime.Now.Date;
                var dbSensorSeries = GetOrCreateEntity(_dbContext.DsSensorLowresDataSet, FirstOfMonth(day), zone.Key);

                void ReadAndSaveSensor(Sensor sensor, TimeSeries<double> seriesToWriteInto, int index)
                {
                    if (!zone.Value.ContainsKey(sensor))
                        return;

                    var resampler = new TimeSeriesResampler<TimeSeries<double>, double>(seriesToWriteInto.Span)
                    {
                        Resampled = seriesToWriteInto
                    };
                    resampler.SampleAggregate(zone.Value[sensor], x => x.Average());

                    dbSensorSeries.SetSeries(index, resampler.Resampled);
                }

                ReadAndSaveSensor(SensorType.TemperatureIndoors, dbSensorSeries.TemperatureSeries, 0);
                ReadAndSaveSensor(SensorType.HumidityIndoors, dbSensorSeries.HumiditySeries, 1);
            }
        }

        private T GetOrCreateEntity<T>(DbSet<T> set, DateTime day, Zone zoneId) where T : DigitalstromZoneSensorData
        {
            int zoneIdInt = zoneId;
            var dbSensorSeries = set.Where(x => x.ZoneId == zoneIdInt && x.Key == day).FirstOrDefault();
            if (dbSensorSeries != null)
                return dbSensorSeries;
            
            var dbZone = _dbContext.DsZones.Where(x => x.Id == zoneIdInt).FirstOrDefault();
            if (dbZone == null)
                _dbContext.DsZones.Add(dbZone = new DigitalstromZone() { Id = zoneIdInt });

            dbSensorSeries = Activator.CreateInstance<T>();
            dbSensorSeries.ZoneId = zoneIdInt;
            dbSensorSeries.Zone = dbZone;
            dbSensorSeries.Key = day;
            set.Add(dbSensorSeries);
            return dbSensorSeries;
        }
    }
}