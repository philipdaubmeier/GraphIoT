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
                            timeseries[timestampedValue.Key.ToLocalTime()] = (int)timestampedValue.Value;

                SaveHighResEnergyValuesToDb(timeseriesCollections);
                SaveMidResEnergyValuesToDb(timeseriesCollections);

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

        public void GenerateMidResEnergySeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections = null;
                try
                {
                    SaveMidResEnergyValuesToDb(timeseriesCollections = ReadHighResEnergyValuesFromDb(new List<DateTime>() { day }));
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
                var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Day == day).FirstOrDefault();
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
                var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Day == collection.Key).FirstOrDefault();
                if (dbEnergySeries == null)
                    _dbContext.DsEnergyHighresDataSet.Add(dbEnergySeries = new DigitalstromEnergyHighresData() { Day = collection.Key });

                dbEnergySeries.EnergySeriesEveryMeter = collection.Value;
            }
        }

        private void SaveMidResEnergyValuesToDb(Dictionary<DateTime, TimeSeriesStreamCollection<Dsuid, int>> timeseriesCollections)
        {
            foreach (var collection in timeseriesCollections)
            {
                var day = collection.Key;
                foreach (var timeseries in collection.Value)
                {
                    var circuitDsuid = (string)timeseries.Key;
                    var dbEnergySeries = _dbContext.DsEnergyMidresDataSet.Where(x => x.CircuitId == circuitDsuid && x.Day == day).FirstOrDefault();
                    if (dbEnergySeries == null)
                    {
                        var dbCircuit = _dbContext.DsCircuits.Where(x => x.Dsuid == circuitDsuid).FirstOrDefault();
                        if (dbCircuit == null)
                            _dbContext.DsCircuits.Add(dbCircuit = new DigitalstromCircuit() { Dsuid = circuitDsuid });

                        _dbContext.DsEnergyMidresDataSet.Add(dbEnergySeries = new DigitalstromEnergyMidresData() { CircuitId = circuitDsuid, Circuit = dbCircuit, Day = day });
                    }

                    var seriesToWriteInto = dbEnergySeries.EnergySeries;
                    var seriesToResample = timeseries.Value;

                    var resampler = new TimeSeriesResampler<TimeSeries<int>, int>(seriesToWriteInto.Span)
                    {
                        Resampled = seriesToWriteInto
                    };
                    resampler.SampleAggregate(seriesToResample, x => (int)x.Average());

                    dbEnergySeries.EnergySeries = resampler.Resampled;
                }
            }
        }
    }
}