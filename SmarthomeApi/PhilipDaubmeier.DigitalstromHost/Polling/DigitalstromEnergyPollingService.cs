using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromHost.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromHost.Polling
{
    public class DigitalstromEnergyPollingService : IDigitalstromPollingService
    {
        private readonly ILogger _logger;
        private readonly IDigitalstromDbContext _dbContext;
        private readonly DigitalstromClient.DigitalstromDssClient _dsClient;

        public DigitalstromEnergyPollingService(ILogger<DigitalstromEnergyPollingService> logger, IDigitalstromDbContext databaseContext, DigitalstromClient.DigitalstromDssClient dsClient)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _dsClient = dsClient;
        }

        public async Task PollValues()
        {
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
        
        private async Task PollEnergyValues()
        {
            var dsuids = (await _dsClient.GetMeteringCircuits()).FilteredMeterNames.Select(x => x.Key).ToList();
            
            var fetchLastValues = (int)TimeSeriesSpan.Spacing.Spacing10Min;
            var days = new TimeSeriesSpan(DateTime.Now.AddSeconds(-1 * fetchLastValues), TimeSeriesSpan.Spacing.Spacing1Sec, fetchLastValues).IncludedDates();

            Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections = null;
            try
            {
                timeseriesCollections = ReadEnergyValuesFromDb(days, dsuids);

                foreach (var dsuid in dsuids)
                    foreach (var timestampedValue in (await _dsClient.GetEnergy(dsuid, (int)TimeSeriesSpan.Spacing.Spacing1Sec, fetchLastValues)).TimeSeries)
                        foreach (var timeseries in timeseriesCollections.Select(x => x.Value[dsuid]))
                            timeseries[timestampedValue.Key.ToLocalTime()] = (int)timestampedValue.Value;

                SaveEnergyValuesToDb(timeseriesCollections);
            }
            catch { throw; }
            finally
            {
                if (timeseriesCollections != null)
                    foreach (var collection in timeseriesCollections)
                        collection.Value.Dispose();
            }
        }

        private Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> ReadEnergyValuesFromDb(IEnumerable<DateTime> days, List<Dsuid> dsuids)
        {
            var timeseriesCollections = new Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>>();
            
            foreach (var day in days)
            {
                var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Day == day).FirstOrDefault();
                if (dbEnergySeries == null)
                    timeseriesCollections.Add(day, DigitalstromEnergyHighresData.InitialEnergySeriesEveryMeter(day, dsuids));
                else
                    timeseriesCollections.Add(day, dbEnergySeries.EnergySeriesEveryMeter);
            }
            return timeseriesCollections;
        }

        private void SaveEnergyValuesToDb(Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections)
        {
            foreach (var collection in timeseriesCollections)
            {
                var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Day == collection.Key).FirstOrDefault();
                if (dbEnergySeries == null)
                    _dbContext.DsEnergyHighresDataSet.Add(dbEnergySeries = new DigitalstromEnergyHighresData() { Day = collection.Key });

                dbEnergySeries.EnergySeriesEveryMeter = collection.Value;
            }

            _dbContext.SaveChanges();
        }
    }
}