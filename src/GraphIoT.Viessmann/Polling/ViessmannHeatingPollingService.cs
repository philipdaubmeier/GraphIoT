using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.Viessmann.Config;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using PhilipDaubmeier.ViessmannClient;
using PhilipDaubmeier.ViessmannClient.Model.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Polling
{
    public class ViessmannHeatingPollingService : IViessmannPollingService
    {
        private readonly ILogger _logger;
        private readonly IViessmannDbContext _dbContext;
        private readonly ViessmannConfig _config;
        private readonly ViessmannPlatformClient _platformClient;

        public ViessmannHeatingPollingService(ILogger<ViessmannHeatingPollingService> logger, IViessmannDbContext dbContext, IOptions<ViessmannConfig> config, ViessmannPlatformClient platformClient)
        {
            _logger = logger;
            _dbContext = dbContext;
            _config = config.Value;
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
            var features = await _platformClient.GetDeviceFeatures(_config.InstallationId, _config.GatewayId);

            var time = DateTime.UtcNow;

            var burnerHoursTotal = (double)features.GetHeatingBurnerStatisticsHours();
            var burnerStartsTotal = (int)features.GetHeatingBurnerStatisticsStarts();
            var burnerModulation = features.GetHeatingBurnerModulation();

            var outsideTemp = features.GetHeatingSensorsTemperatureOutside();
            var boilerTemp = features.GetHeatingBoilerTemperature();
            var boilerTempMain = features.GetHeatingBoilerSensorsTemperatureMain();
            var circuit0Temp = features.GetHeatingCircuitsSensorsTemperature(FeatureName.Circuit.Circuit0);
            var circuit1Temp = features.GetHeatingCircuitsSensorsTemperature(FeatureName.Circuit.Circuit1);
            var dhwTemp = features.GetHeatingDhwSensorsTemperatureHotWaterStorage();

            var burnerActive = features.IsHeatingBurnerActive();
            var circuit0Pump = features.IsHeatingCircuitsCirculationPumpOn(FeatureName.Circuit.Circuit0);
            var circuit1Pump = features.IsHeatingCircuitsCirculationPumpOn(FeatureName.Circuit.Circuit1);
            var dhwPrimPump = features.IsHeatingDhwPumpsPrimaryOn();
            var dhwCircPump = features.IsHeatingDhwPumpsCirculationOn();

            var solarWhTotal = features.GetHeatingSolarPowerProductionWhToday();
            var solarCollectorTemp = features.GetHeatingSolarSensorsTemperatureCollector();
            var solarHotwaterTemp = features.GetHeatingSolarSensorsTemperatureDhw();
            var solarPumpState = features.IsHeatingSolarPumpsCircuitOn();
            var solarSuppression = features.IsHeatingSolarRechargeSuppressionOn();

            SaveHeatingValues(time, burnerHoursTotal, burnerStartsTotal, burnerModulation, outsideTemp, boilerTemp, boilerTempMain, circuit0Temp, circuit1Temp, dhwTemp, burnerActive, circuit0Pump, circuit1Pump, dhwPrimPump, dhwCircPump);

            SaveSolarValues(time, solarWhTotal, solarCollectorTemp, solarHotwaterTemp, solarPumpState, solarSuppression);
        }

        private void SaveHeatingValues(DateTime time, double burnerHoursTotal, int burnerStartsTotal, int burnerModulation, double outsideTemp, double boilerTemp, double boilerTempMain, double circuit0Temp, double circuit1Temp, double dhwTemp, bool burnerActive, bool circuit0Pump, bool circuit1Pump, bool dhwPrimPump, bool dhwCircPump)
        {
            var dbHeatingSeries = TimeSeriesDbEntityBase.LoadOrCreateDay(_dbContext.ViessmannHeatingTimeseries, time.Date);

            var oldHours = dbHeatingSeries.BurnerHoursTotal;
            var minutes = (burnerHoursTotal - oldHours) * 60;
            var series1 = dbHeatingSeries.BurnerMinutesSeries;
            series1.Accumulate(time, minutes > 10 || minutes < 0 ? 0 : minutes);
            dbHeatingSeries.SetSeries(0, series1);
            dbHeatingSeries.BurnerHoursTotal = burnerHoursTotal;

            var oldStarts = dbHeatingSeries.BurnerStartsTotal;
            var startsDiff = burnerStartsTotal - oldStarts;
            var series2 = dbHeatingSeries.BurnerStartsSeries;
            series1.Accumulate(time, startsDiff > 10 || startsDiff < 0 ? 0 : startsDiff);
            dbHeatingSeries.SetSeries(1, series2);
            dbHeatingSeries.BurnerStartsTotal = burnerStartsTotal;

            dbHeatingSeries.SetSeriesValue(2, time, burnerModulation);
            dbHeatingSeries.SetSeriesValue(3, time, outsideTemp);
            dbHeatingSeries.SetSeriesValue(4, time, boilerTemp);
            dbHeatingSeries.SetSeriesValue(5, time, boilerTempMain);
            dbHeatingSeries.SetSeriesValue(6, time, circuit0Temp);
            dbHeatingSeries.SetSeriesValue(7, time, circuit1Temp);
            dbHeatingSeries.SetSeriesValue(8, time, dhwTemp);
            dbHeatingSeries.SetSeriesValue(9, time, burnerActive);
            dbHeatingSeries.SetSeriesValue(10, time, circuit0Pump);
            dbHeatingSeries.SetSeriesValue(11, time, circuit1Pump);
            dbHeatingSeries.SetSeriesValue(12, time, dhwPrimPump);
            dbHeatingSeries.SetSeriesValue(13, time, dhwCircPump);

            SaveLowresHeatingValues(time.Date, dbHeatingSeries);

            _dbContext.SaveChanges();
        }

        public void GenerateLowResHeatingSeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                var dbHeatingSeries = _dbContext.ViessmannHeatingTimeseries.Where(x => x.Key == day).FirstOrDefault();
                if (dbHeatingSeries == null)
                    continue;

                SaveLowresHeatingValues(day, dbHeatingSeries);
                _dbContext.SaveChanges();
            }
        }

        private void SaveLowresHeatingValues(DateTime day, ViessmannHeatingMidresData midRes)
        {
            TimeSeriesDbEntityBase.LoadOrCreateMonth(_dbContext.ViessmannHeatingLowresTimeseries, day).ResampleFromAll(midRes);
        }

        private void SaveSolarValues(DateTime time, int solarWhTotal, double solarCollectorTemp, double solarHotwaterTemp, bool solarPumpState, bool solarSuppression)
        {
            var dbSolarSeries = TimeSeriesDbEntityBase.LoadOrCreateDay(_dbContext.ViessmannSolarTimeseries, time.Date);

            var oldSolarWhTotal = dbSolarSeries.SolarWhTotal;
            var solarWhDiff = oldSolarWhTotal.HasValue ? solarWhTotal - oldSolarWhTotal.Value : 0;
            var series1 = dbSolarSeries.SolarWhSeries;
            series1.Accumulate(time - series1.Span.Duration, solarWhDiff / 2);
            series1.Accumulate(time, solarWhDiff / 2);
            dbSolarSeries.SetSeries(0, series1);
            dbSolarSeries.SolarWhTotal = solarWhTotal;

            dbSolarSeries.SetSeriesValue(1, time, solarCollectorTemp);
            dbSolarSeries.SetSeriesValue(2, time, solarHotwaterTemp);
            dbSolarSeries.SetSeriesValue(3, time, solarPumpState);
            dbSolarSeries.SetSeriesValue(4, time, solarSuppression);

            SaveLowresSolarValues(_dbContext, time.Date, dbSolarSeries);

            _dbContext.SaveChanges();
        }

        public void GenerateLowResSolarSeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                var dbSolarSeries = _dbContext.ViessmannSolarTimeseries.Where(x => x.Key == day).FirstOrDefault();
                if (dbSolarSeries == null)
                    continue;

                SaveLowresSolarValues(_dbContext, day, dbSolarSeries);
                _dbContext.SaveChanges();
            }
        }

        private static void SaveLowresSolarValues(IViessmannDbContext dbContext, DateTime day, ViessmannSolarMidresData midRes)
        {
            var dbSolarSeries = TimeSeriesDbEntityBase.LoadOrCreateMonth(dbContext.ViessmannSolarLowresTimeseries, day);

            // Hack: remove first 5 elements due to bug in day-boundaries
            static ITimeSeries<int> PreprocessSolarProduction(ITimeSeries<int> input)
            {
                for (int i = 0; i < 5; i++)
                    input[i] = input[i].HasValue ? (int?)0 : null;
                return input;
            }

            dbSolarSeries.ResampleFrom<int>(midRes, 0, x => (int)x.Average(), PreprocessSolarProduction);
            dbSolarSeries.ResampleFromAll(midRes, 0);
        }
    }
}