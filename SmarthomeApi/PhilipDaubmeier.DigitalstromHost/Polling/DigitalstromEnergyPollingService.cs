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
    public class DigitalstromEnergyPollingService : IDigitalstromPollingService
    {
        private readonly ILogger _logger;
        private readonly IDigitalstromDbContext _dbContext;
        private readonly DigitalstromDssClient _dsClient;
        private readonly IDigitalstromStructureService _dsStructure;

        private List<Dsuid> Dsuids => _dsStructure.Circuits.Where(x => _dsStructure.IsMeteringCircuit(x)).ToList();

        public DigitalstromEnergyPollingService(ILogger<DigitalstromEnergyPollingService> logger, IDigitalstromDbContext databaseContext, DigitalstromDssClient dsClient, IDigitalstromStructureService digitalstromStructure)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _dsClient = dsClient;
            _dsStructure = digitalstromStructure;
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
            var fetchLastValues = (int)TimeSeriesSpan.Spacing.Spacing10Min;
            var days = new TimeSeriesSpan(DateTime.Now.AddSeconds(-1 * fetchLastValues), TimeSeriesSpan.Spacing.Spacing1Sec, fetchLastValues).IncludedDates();

            Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections = null;
            try
            {
                timeseriesCollections = ReadHighResEnergyValuesFromDb(days);

                foreach (var dsuid in Dsuids)
                    foreach (var timestampedValue in (await _dsClient.GetEnergy(dsuid, (int)TimeSeriesSpan.Spacing.Spacing1Sec, fetchLastValues)).TimeSeries)
                        foreach (var timeseries in timeseriesCollections.Select(x => x.Value[dsuid]))
                            timeseries[timestampedValue.Key.ToUniversalTime()] = (int)timestampedValue.Value;

                SaveHighResEnergyValuesToDb(timeseriesCollections);
                SaveMidResEnergyValuesToDb(timeseriesCollections);
                SaveLowResEnergyValuesToDb(timeseriesCollections);

                _dbContext.SaveChanges();
            }
            catch { throw; }
            finally
            {
                if (timeseriesCollections != null)
                    foreach (var collection in timeseriesCollections)
                        collection.Value.Dispose();
            }
        }

        public void GenerateMidLowResEnergySeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections = null;
                try
                {
                    timeseriesCollections = ReadHighResEnergyValuesFromDb(new List<DateTime>() { day });
                    SaveMidResEnergyValuesToDb(timeseriesCollections);
                    SaveLowResEnergyValuesToDb(timeseriesCollections);
                    _dbContext.SaveChanges();
                }
                catch { throw; }
                finally { timeseriesCollections?.FirstOrDefault().Value?.Dispose(); }
            }
        }

        private Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> ReadHighResEnergyValuesFromDb(IEnumerable<DateTime> days)
        {
            var timeseriesCollections = new Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>>();
            
            foreach (var day in days)
            {
                var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Key == day).FirstOrDefault();
                if (dbEnergySeries == null)
                    timeseriesCollections.Add(day, DigitalstromEnergyHighresData.InitialEnergySeriesEveryMeter(day, Dsuids));
                else
                    timeseriesCollections.Add(day, dbEnergySeries.EnergySeriesEveryMeter);
            }
            return timeseriesCollections;
        }

        private void SaveHighResEnergyValuesToDb(Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections)
        {
            foreach (var collection in timeseriesCollections)
            {
                var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Key == collection.Key).FirstOrDefault();
                if (dbEnergySeries == null)
                    _dbContext.DsEnergyHighresDataSet.Add(dbEnergySeries = new DigitalstromEnergyHighresData() { Key = collection.Key });

                dbEnergySeries.EnergySeriesEveryMeter = collection.Value;
            }
        }

        private void SaveMidResEnergyValuesToDb(Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections)
        {
            SaveMidLowResEnergyValuesToDb(timeseriesCollections.GroupBy(x => x.Key), db => db.DsEnergyMidresDataSet);
        }

        private void SaveLowResEnergyValuesToDb(Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections)
        {
            DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
            SaveMidLowResEnergyValuesToDb(timeseriesCollections.GroupBy(x => FirstOfMonth(x.Key)), db => db.DsEnergyLowresDataSet);
        }

        private void SaveMidLowResEnergyValuesToDb<T>(IEnumerable<IGrouping<DateTime, KeyValuePair<DateTime, TimeSeriesStreamCollection<Dsuid, int>>>> groupedCollections,
            Func<IDigitalstromDbContext, DbSet<T>> dbSetSelector) where T : DigitalstromEnergyData
        {
            foreach (var collection in groupedCollections)
            {
                foreach (var timeseries in collection.SelectMany(x => x.Value).GroupBy(x => x.Key))
                {
                    var circuitDsuid = (string)timeseries.Key;
                    var dbEnergySeries = dbSetSelector(_dbContext).Where(x => x.CircuitId == circuitDsuid && x.Key == collection.Key).FirstOrDefault();
                    if (dbEnergySeries == null)
                    {
                        var dbCircuit = _dbContext.DsCircuits.Where(x => x.Dsuid == circuitDsuid).FirstOrDefault();
                        if (dbCircuit == null)
                            _dbContext.DsCircuits.Add(dbCircuit = new DigitalstromCircuit() { Dsuid = circuitDsuid });

                        dbEnergySeries = Activator.CreateInstance<T>();
                        dbEnergySeries.Key = collection.Key;
                        dbEnergySeries.Circuit = dbCircuit;
                        dbEnergySeries.CircuitId = circuitDsuid;
                        dbSetSelector(_dbContext).Add(dbEnergySeries);
                    }

                    var seriesToWriteInto = dbEnergySeries.EnergySeries;
                    var resampler = new TimeSeriesResampler<TimeSeries<int>, int>(seriesToWriteInto.Span)
                    {
                        Resampled = seriesToWriteInto
                    };
                    resampler.SampleAggregate(timeseries.Select(x => x.Value), x => (int)x.Average());

                    dbEnergySeries.SetSeries(0, resampler.Resampled);
                }
            }
        }
    }
}