using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmarthomeApi.Clients.Viessmann;
using SmarthomeApi.Database.Model;
using SmarthomeApi.Model.Config;

namespace SmarthomeApi.Services
{
    public class ViessmannTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly PersistenceContext _dbContext;
        private readonly ViessmannPlatformClient _platformClient;
        private readonly ViessmannVitotrolClient _vitotrolClient;
        private Timer _timer;

        public ViessmannTimedHostedService(ILogger<ViessmannTimedHostedService> logger, PersistenceContext databaseContext, IOptions<ViessmannConfig> config)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _vitotrolClient = new ViessmannVitotrolClient(_dbContext, config);
            _platformClient = new ViessmannPlatformClient(_dbContext, config);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Viessmann Background Service is starting.");

            _timer = new Timer(PollAll, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Viessmann Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async void PollAll(object state)
        {
            _logger.LogInformation($"{DateTime.Now} Viessmann Background Service is polling new heating values...");
            
            try
            {
                await PollHeatingValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in viessmann heating background worker: {ex.Message}");
            }

            _logger.LogInformation($"{DateTime.Now} Viessmann Background Service is polling new solar values...");
            
            try
            {
                await PollSolarValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in viessmann solar background worker: {ex.Message}");
            }
        }

        private async Task PollHeatingValues()
        {
            var burnerStatistics = await _platformClient.GetBurnerStatistics();
            var burnerHoursTotal = burnerStatistics.Item1;
            var burnerStartsTotal = burnerStatistics.Item2;
            var burnerModulation = await _platformClient.GetBurnerModulation();

            var outsideTemp = (await _platformClient.GetOutsideTemperature()).Item2;
            var boilerTemp = await _platformClient.GetBoilerTemperature();
            var boilerTempMain = (await _platformClient.GetBoilerTemperatureMain()).Item2;
            var circuit0Temp = (await _platformClient.GetCircuitTemperature(ViessmannPlatformClient.Circuit.Circuit0)).Item2;
            var circuit1Temp = (await _platformClient.GetCircuitTemperature(ViessmannPlatformClient.Circuit.Circuit1)).Item2;
            var dhwTemp = (await _platformClient.GetDhwStorageTemperature()).Item2;

            var burnerActive = await _platformClient.GetBurnerActiveStatus();
            var circuit0Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit1Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit1);
            var dhwPrimPump = await _platformClient.GetDhwPrimaryPump();
            var dhwCircPump = await _platformClient.GetDhwCirculationPump();

            var time = DateTime.Now;
            var day = time.Date;

            _dbContext.Semaphore.WaitOne();
            try
            {
                var dbHeatingSeries = _dbContext.ViessmannHeatingTimeseries.Where(x => x.Day == day).FirstOrDefault();
                if (dbHeatingSeries == null)
                    _dbContext.ViessmannHeatingTimeseries.Add(dbHeatingSeries = new ViessmannHeatingData() { Day = day, BurnerHoursTotal = 0d, BurnerStartsTotal = 0 });

                var oldHours = dbHeatingSeries.BurnerHoursTotal;
                var minutes = (burnerHoursTotal - oldHours) * 60;
                var series1 = dbHeatingSeries.BurnerMinutesSeries;
                series1.Accumulate(time, minutes > 10 || minutes < 0 ? 0 : minutes);
                dbHeatingSeries.BurnerMinutesSeries = series1;
                dbHeatingSeries.BurnerHoursTotal = burnerHoursTotal;

                var oldStarts = dbHeatingSeries.BurnerStartsTotal;
                var startsDiff = burnerStartsTotal - oldStarts;
                var series2 = dbHeatingSeries.BurnerStartsSeries;
                series1.Accumulate(time, startsDiff > 10 || startsDiff < 0 ? 0 : startsDiff);
                dbHeatingSeries.BurnerStartsSeries = series2;
                dbHeatingSeries.BurnerStartsTotal = burnerStartsTotal;

                var series3 = dbHeatingSeries.BurnerModulationSeries;
                series3[time] = burnerModulation;
                dbHeatingSeries.BurnerModulationSeries = series3;

                var series4 = dbHeatingSeries.OutsideTempSeries;
                series4[time] = outsideTemp;
                dbHeatingSeries.OutsideTempSeries = series4;

                var series5 = dbHeatingSeries.BoilerTempSeries;
                series5[time] = boilerTemp;
                dbHeatingSeries.BoilerTempSeries = series5;

                var series6 = dbHeatingSeries.BoilerTempMainSeries;
                series6[time] = boilerTempMain;
                dbHeatingSeries.BoilerTempMainSeries = series6;

                var series7 = dbHeatingSeries.Circuit0TempSeries;
                series7[time] = circuit0Temp;
                dbHeatingSeries.Circuit0TempSeries = series7;

                var series8 = dbHeatingSeries.Circuit1TempSeries;
                series8[time] = circuit1Temp;
                dbHeatingSeries.Circuit1TempSeries = series8;

                var series9 = dbHeatingSeries.DhwTempSeries;
                series9[time] = dhwTemp;
                dbHeatingSeries.DhwTempSeries = series9;

                var series10 = dbHeatingSeries.BurnerActiveSeries;
                series10[time] = burnerActive;
                dbHeatingSeries.BurnerActiveSeries = series10;

                var series11 = dbHeatingSeries.Circuit0PumpSeries;
                series11[time] = circuit0Pump;
                dbHeatingSeries.Circuit0PumpSeries = series11;

                var series12 = dbHeatingSeries.Circuit1PumpSeries;
                series12[time] = circuit1Pump;
                dbHeatingSeries.Circuit1PumpSeries = series12;

                var series13 = dbHeatingSeries.DhwPrimaryPumpSeries;
                series13[time] = dhwPrimPump;
                dbHeatingSeries.DhwPrimaryPumpSeries = series13;

                var series14 = dbHeatingSeries.DhwCirculationPumpSeries;
                series14[time] = dhwCircPump;
                dbHeatingSeries.DhwCirculationPumpSeries = series14;

                _dbContext.SaveChanges();
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }

        private async Task PollSolarValues()
        {
            var data = await _vitotrolClient.GetData(new List<int>() { 5272, 5273, 5274, 5276, 7895 });
            var time = data.First().Item3;
            var day = time.Date;

            _dbContext.Semaphore.WaitOne();
            try
            {
                var dbSolarSeries = _dbContext.ViessmannSolarTimeseries.Where(x => x.Day == day).FirstOrDefault();
                if (dbSolarSeries == null)
                    _dbContext.ViessmannSolarTimeseries.Add(dbSolarSeries = new ViessmannSolarData() { Day = day });

                var item = data.First(d => d.Item1 == 7895.ToString());
                var newValue = int.Parse(item.Item2);
                var oldValue = dbSolarSeries.SolarWhTotal.HasValue ? dbSolarSeries.SolarWhTotal.Value : 0;
                var series1 = dbSolarSeries.SolarWhSeries;
                series1.Accumulate(time, newValue - oldValue);
                dbSolarSeries.SolarWhSeries = series1;
                dbSolarSeries.SolarWhTotal = newValue;

                item = data.First(d => d.Item1 == 5272.ToString());
                var series2 = dbSolarSeries.SolarCollectorTempSeries;
                series2[time] = double.Parse(item.Item2, CultureInfo.InvariantCulture);
                dbSolarSeries.SolarCollectorTempSeries = series2;

                item = data.First(d => d.Item1 == 5276.ToString());
                var series3 = dbSolarSeries.SolarHotwaterTempSeries;
                series3[time] = double.Parse(item.Item2, CultureInfo.InvariantCulture);
                dbSolarSeries.SolarHotwaterTempSeries = series3;

                item = data.First(d => d.Item1 == 5274.ToString());
                var series4 = dbSolarSeries.SolarPumpStateSeries;
                series4[time] = item.Item2.Trim() != "0";
                dbSolarSeries.SolarPumpStateSeries = series4;

                item = data.First(d => d.Item1 == 5273.ToString());
                var series5 = dbSolarSeries.SolarSuppressionSeries;
                series5[time] = item.Item2.Trim() != "0";
                dbSolarSeries.SolarSuppressionSeries = series5;

                _dbContext.SaveChanges();
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }
    }
}
