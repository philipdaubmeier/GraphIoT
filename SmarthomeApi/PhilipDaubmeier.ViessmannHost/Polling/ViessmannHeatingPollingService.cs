using Microsoft.Extensions.Logging;
using PhilipDaubmeier.ViessmannClient;
using PhilipDaubmeier.ViessmannHost.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannHost.Polling
{
    public class ViessmannHeatingPollingService : IViessmannPollingService
    {
        private readonly ILogger _logger;
        private readonly IViessmannDbContext _dbContext;
        private readonly ViessmannPlatformClient _platformClient;

        public ViessmannHeatingPollingService(ILogger<ViessmannHeatingPollingService> logger, IViessmannDbContext dbContext, ViessmannPlatformClient platformClient)
        {
            _logger = logger;
            _dbContext = dbContext;
            _platformClient = platformClient;
        }

        public async Task PollValues()
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
    }
}