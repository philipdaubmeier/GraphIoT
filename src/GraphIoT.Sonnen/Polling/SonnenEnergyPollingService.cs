using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Sonnen.Database;
using PhilipDaubmeier.SonnenClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Polling
{
    public class SonnenEnergyPollingService : ISonnenPollingService
    {
        private readonly ILogger _logger;
        private readonly ISonnenDbContext _dbContext;
        private readonly SonnenPortalClient _sonnenClient;

        private string? _siteId = null;

        public SonnenEnergyPollingService(ILogger<SonnenEnergyPollingService> logger, ISonnenDbContext databaseContext, SonnenPortalClient sonnenClient)
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
                await PollSensorValues(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in MySonnen energy background worker: {ex.Message}");
            }
        }

        public async Task PollSensorValues(DateTime start, DateTime end)
        {
            if (string.IsNullOrEmpty(_siteId))
            {
                _siteId = (await _sonnenClient.GetUserSites()).DefaultSiteId;
                if (string.IsNullOrEmpty(_siteId))
                    return;
            }

            var energyValues = await _sonnenClient.GetEnergyMeasurements(_siteId, start, end);
            if (energyValues.Start == DateTime.MinValue || energyValues.End == DateTime.MinValue)
                return;

            static TimeSeries<T> ToTimeSeries<T>(TimeSeriesSpan span, List<T?> source) where T : struct
            {
                var series = new TimeSeries<T>(span);
                for (int i = 0; i < Math.Min(span.Count, source.Count); i++)
                    series[i] = source[i];
                return series;
            }

            var span = new TimeSeriesSpan(energyValues.Start.ToUniversalTime(), energyValues.End.ToUniversalTime(), energyValues.Resolution);
            var productionPower = ToTimeSeries(span, energyValues.ProductionPower);
            var consumptionPower = ToTimeSeries(span, energyValues.ConsumptionPower);
            var directUsagePower = ToTimeSeries(span, energyValues.DirectUsagePower);
            var batteryCharging = ToTimeSeries(span, energyValues.BatteryCharging);
            var batteryDischarging = ToTimeSeries(span, energyValues.BatteryDischarging);
            var gridFeedin = ToTimeSeries(span, energyValues.GridFeedin);
            var gridPurchase = ToTimeSeries(span, energyValues.GridPurchase);
            var batteryUsoc = ToTimeSeries(span, energyValues.BatteryUsoc);

            foreach (var day in span.IncludedDates())
            {
                SaveEnergyMidResValues(day, productionPower, consumptionPower, directUsagePower,
                    batteryCharging, batteryDischarging, gridFeedin, gridPurchase, batteryUsoc);
            }

            _dbContext.SaveChanges();
        }

        private void SaveEnergyMidResValues(DateTime day, TimeSeries<int> productionPower, TimeSeries<int> consumptionPower,
            TimeSeries<int> directUsagePower, TimeSeries<int> batteryCharging, TimeSeries<int> batteryDischarging,
            TimeSeries<int> gridFeedin, TimeSeries<int> gridPurchase, TimeSeries<double> batteryUsoc)
        {
            var dbEnergySeries = _dbContext.SonnenEnergyDataSet.Where(x => x.Key == day).FirstOrDefault();
            if (dbEnergySeries == null)
                _dbContext.SonnenEnergyDataSet.Add(dbEnergySeries = new SonnenEnergyMidresData() { Key = day });

            dbEnergySeries.CopyInto(0, productionPower);
            dbEnergySeries.CopyInto(1, consumptionPower);
            dbEnergySeries.CopyInto(2, directUsagePower);
            dbEnergySeries.CopyInto(3, batteryCharging);
            dbEnergySeries.CopyInto(4, batteryDischarging);
            dbEnergySeries.CopyInto(5, gridFeedin);
            dbEnergySeries.CopyInto(6, gridPurchase);
            dbEnergySeries.CopyInto(7, batteryUsoc);

            SaveEnergyLowResValues(day, dbEnergySeries);
        }

        private void SaveEnergyLowResValues(DateTime day, SonnenEnergyMidresData midRes)
        {
            static DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
            var month = FirstOfMonth(day);
            var dbEnergySeries = _dbContext.SonnenEnergyLowresDataSet.Where(x => x.Key == month).FirstOrDefault();
            if (dbEnergySeries == null)
                _dbContext.SonnenEnergyLowresDataSet.Add(dbEnergySeries = new SonnenEnergyLowresData() { Key = month });

            foreach (var i in Enumerable.Range(0, 7))
                dbEnergySeries.ResampleFrom<int>(midRes, i, x => (int)x.Average());

            dbEnergySeries.ResampleFrom<double>(midRes, 7, x => x.Average());
        }
    }
}