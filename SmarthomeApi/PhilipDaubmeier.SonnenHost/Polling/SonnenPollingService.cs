using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenHost.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SonnenHost.Polling
{
    public class SonnenPollingService : ISonnenPollingService
    {
        private readonly ILogger _logger;
        private readonly ISonnenDbContext _dbContext;
        private readonly SonnenPortalClient _sonnenClient;

        private string _siteId = null;

        public SonnenPollingService(ILogger<SonnenPollingService> logger, ISonnenDbContext databaseContext, SonnenPortalClient sonnenClient)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _sonnenClient = sonnenClient;
        }

        public async Task PollValues()
        {
            _logger.LogInformation($"{DateTime.Now} MySonnen Background Service is polling new energy values...");
            
            try
            {
                await PollSensorValues(DateTime.Now.AddHours(-1), DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in MySonnen energy background worker: {ex.Message}");
            }
        }

        private async Task PollSensorValues(DateTime start, DateTime end)
        {
            if (string.IsNullOrEmpty(_siteId))
            {
                _siteId = (await _sonnenClient.GetUserSites()).DefaultSiteId;
                if (string.IsNullOrEmpty(_siteId))
                    return;
            }

            var energyValues = await _sonnenClient.GetEnergyMeasurements(_siteId, start, end);
            if (!energyValues.Start.HasValue || !energyValues.End.HasValue)
                return;

            var span = new TimeSeriesSpan(energyValues.Start.Value, energyValues.End.Value, energyValues.Resolution);
            var productionPower = new TimeSeries<int>(span);
            var consumptionPower = new TimeSeries<int>(span);
            var directUsagePower = new TimeSeries<int>(span);
            var batteryCharging = new TimeSeries<int>(span);
            var batteryDischarging = new TimeSeries<int>(span);
            var gridFeedin = new TimeSeries<int>(span);
            var gridPurchase = new TimeSeries<int>(span);
            var batteryUsoc = new TimeSeries<double>(span);

            for (int i = 0; i < Math.Min(span.Count, energyValues?.ProductionPower?.Count ?? 0); i++)
            {
                productionPower[i] = energyValues.ProductionPower[i];
                consumptionPower[i] = energyValues.ConsumptionPower[i];
                directUsagePower[i] = energyValues.DirectUsagePower[i];
                batteryCharging[i] = energyValues.BatteryCharging[i];
                batteryDischarging[i] = energyValues.BatteryDischarging[i];
                gridFeedin[i] = energyValues.GridFeedin[i];
                gridPurchase[i] = energyValues.GridPurchase[i];
                batteryUsoc[i] = energyValues.BatteryUsoc[i];
            }

            foreach (var day in span.IncludedDates())
                SaveEnergyValues(day, productionPower, consumptionPower, directUsagePower,
                    batteryCharging, batteryDischarging, gridFeedin, gridPurchase, batteryUsoc);

            _dbContext.SaveChanges();
        }
        
        private void SaveEnergyValues(DateTime day, TimeSeries<int> productionPower, TimeSeries<int> consumptionPower, 
            TimeSeries<int> directUsagePower, TimeSeries<int> batteryCharging, TimeSeries<int> batteryDischarging, 
            TimeSeries<int> gridFeedin, TimeSeries<int> gridPurchase, TimeSeries<double> batteryUsoc)
        {
            var dbEnergySeries = _dbContext.SonnenEnergyDataSet.Where(x => x.Day == day).FirstOrDefault();
            if (dbEnergySeries == null)
                _dbContext.SonnenEnergyDataSet.Add(dbEnergySeries = new SonnenEnergyData() { Day = day });

            var productionPowerSeries = dbEnergySeries.ProductionPowerSeries;
            var consumptionPowerSeries = dbEnergySeries.ConsumptionPowerSeries;
            var directUsagePowerSeries = dbEnergySeries.DirectUsagePowerSeries;
            var batteryChargingSeries = dbEnergySeries.BatteryChargingSeries;
            var batteryDischargingSeries = dbEnergySeries.BatteryDischargingSeries;
            var gridFeedinSeries = dbEnergySeries.GridFeedinSeries;
            var gridPurchaseSeries = dbEnergySeries.GridPurchaseSeries;
            var batteryUsocSeries = dbEnergySeries.BatteryUsocSeries;

            for (int i = 0; i < productionPower.Span.Count; i++)
            {
                var time = productionPower.Span.Begin + (i * productionPower.Span.Duration);

                productionPowerSeries[time] = productionPower[time];
                consumptionPowerSeries[time] = consumptionPower[time];
                directUsagePowerSeries[time] = directUsagePower[time];
                batteryChargingSeries[time] = batteryCharging[time];
                batteryDischargingSeries[time] = batteryDischarging[time];
                gridFeedinSeries[time] = gridFeedin[time];
                gridPurchaseSeries[time] = gridPurchase[time];
                batteryUsocSeries[time] = batteryUsoc[time];
            }

            dbEnergySeries.ProductionPowerSeries = productionPowerSeries;
            dbEnergySeries.ConsumptionPowerSeries = consumptionPowerSeries;
            dbEnergySeries.DirectUsagePowerSeries = directUsagePowerSeries;
            dbEnergySeries.BatteryChargingSeries = batteryChargingSeries;
            dbEnergySeries.BatteryDischargingSeries = batteryDischargingSeries;
            dbEnergySeries.GridFeedinSeries = gridFeedinSeries;
            dbEnergySeries.GridPurchaseSeries = gridPurchaseSeries;
            dbEnergySeries.BatteryUsocSeries = batteryUsocSeries;
        }
    }
}