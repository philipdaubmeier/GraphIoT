using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmarthomeApi.Clients.Digitalstrom;
using SmarthomeApi.Database.Model;
using SmarthomeApi.FormatParsers;

namespace SmarthomeApi.Services
{
    public class DigitalstromTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly PersistenceContext _dbContext;
        private readonly DigitalstromClient _dsClient;
        private Timer _timer;

        public DigitalstromTimedHostedService(ILogger<ViessmannTimedHostedService> logger, PersistenceContext databaseContext)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _dsClient = new DigitalstromClient(_dbContext);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is starting.");

            _timer = new Timer(PollAll, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        
        private async void PollAll(object state)
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

            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is polling new energy values...");

            try
            {
                await PollEnergyValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Digitalstrom energy background worker: {ex.Message}");
            }
        }

        private async Task PollSensorValues()
        {
            var sensorValues = (await _dsClient.GetSensorValues()).GroupBy(x => x.Key.Item1);

            _dbContext.Semaphore.WaitOne();
            try
            {
                foreach (var zoneGrouping in sensorValues)
                    SaveZoneSensorValues(zoneGrouping.Key, zoneGrouping.ToDictionary(x => x.Key.Item2, x => x.Value.Item2));

                _dbContext.SaveChanges();
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }

        private async Task PollEnergyValues()
        {
            var dsuids = (await _dsClient.GetMeteringCircuits()).Select(x => x.Key).ToList();
            var energyCurves = new Dictionary<DSUID, TimeSeries<int>>();
            foreach (var dsuid in dsuids)
            {
                var rawSeries = await _dsClient.GetEnergy(dsuid, 1, 10 * 60);
                var timeseries = new TimeSeries<int>(rawSeries.First().Key, rawSeries.Last().Key, rawSeries.Count);
                foreach (var rawValue in rawSeries)
                    timeseries[rawValue.Key] = (int)rawValue.Value;
                energyCurves.Add(new DSUID(dsuid), timeseries);
            }

            var energyCurvesAllDay = new Dictionary<DSUID, TimeSeries<int>>();
            foreach (var dsuid in energyCurves.Keys)
            {
                var timeseries = new TimeSeries<int>(DateTime.Now.Date, DateTime.Now.Date.AddDays(1), 60 * 60 * 24);
                foreach (var value in energyCurves[dsuid])
                    timeseries[value.Key] = value.Value;
                energyCurvesAllDay.Add(dsuid, timeseries);
            }

            byte[] blob = energyCurves.ToCompressedBlob();
            byte[] blob2 = energyCurvesAllDay.ToCompressedBlob();

            _dbContext.Semaphore.WaitOne();
            try
            {
                // TODO: save the blob
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }

        private void SaveZoneSensorValues(int zoneId, Dictionary<int, double> sensorValues)
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
