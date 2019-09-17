using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using PhilipDaubmeier.NetatmoHost.Database;
using PhilipDaubmeier.NetatmoHost.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoHost.Polling
{
    public class NetatmoWeatherPollingService : INetatmoPollingService
    {
        private readonly ILogger _logger;
        private readonly INetatmoDbContext _dbContext;
        private readonly NetatmoWebClient _netatmoClient;
        private readonly INetatmoDeviceService _netatmoStructure;

        public NetatmoWeatherPollingService(ILogger<NetatmoWeatherPollingService> logger, INetatmoDbContext databaseContext, NetatmoWebClient netatmoClient, INetatmoDeviceService netatmoStructure)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _netatmoClient = netatmoClient;
            _netatmoStructure = netatmoStructure;
        }

        public async Task PollValues()
        {
            _logger.LogInformation($"{DateTime.Now} Netatmo Background Service is polling new weather station values...");

            try
            {
                await PollSensorValues(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Netatmo weather background worker: {ex.Message}");
            }
        }

        public async Task PollSensorValues(DateTime start, DateTime end)
        {
            var loadedTimeseries = await LoadMidresSensorValues(start, end);

            var dbReadTimeseries = ReadAndSaveMidresSensorValues(loadedTimeseries);
            _dbContext.SaveChanges();

            SaveLowresSensorValues(dbReadTimeseries);
            _dbContext.SaveChanges();

            _netatmoStructure.RefreshDbGuids();
        }

        private async Task<Dictionary<Tuple<ModuleId, ModuleId, Measure>, List<TimeSeries<double>>>> LoadMidresSensorValues(DateTime start, DateTime end)
        {
            var days = new TimeSeriesSpan(start.ToUniversalTime(), end.ToUniversalTime(), 1).IncludedDates().ToList();
            var loadedValues = new Dictionary<Tuple<ModuleId, ModuleId, Measure>, List<TimeSeries<double>>>();
            foreach (var module in _netatmoStructure.Modules)
            {
                var measureTypes = _netatmoStructure.GetModuleMeasures(module.Item2);
                var measures = await _netatmoClient.GetMeasure(module.Item1, module.Item2, measureTypes, MeasureScale.ScaleMax, start, end, null, true);

                var series = measureTypes.Where(m => m != null).ToDictionary(m => m, m => Enumerable.Range(0, days.Count).Select(k =>
                        new TimeSeries<double>(new TimeSeriesSpan(days[k], days[k].AddDays(1), TimeSeriesSpan.Spacing.Spacing5Min))).ToList());
                foreach (var measure in measures)
                    foreach (var originSeries in measure.Value)
                        for (int k = 0; k < series[measure.Key].Count; k++)
                            MapToEquidistantTimeSeries(series[measure.Key][k], originSeries);
                foreach (var serie in series)
                    loadedValues.Add(new Tuple<ModuleId, ModuleId, Measure>(module.Item1, module.Item2, serie.Key), serie.Value);
            }
            return loadedValues;
        }

        private void MapToEquidistantTimeSeries<T>(ITimeSeries<T> series, KeyValuePair<DateTime, T> valueToMap) where T : struct
        {
            if (valueToMap.Key.ToUniversalTime() < series.Span.Begin.ToUniversalTime() || valueToMap.Key.ToUniversalTime() > series.Span.End.ToUniversalTime())
                return;

            int findBestIndex()
            {
                var indexDouble = (valueToMap.Key.ToUniversalTime() - series.Span.Begin.ToUniversalTime()) / series.Span.Duration;
                var index = Math.Min(series.Count - 1, Math.Max(0, (int)Math.Floor(indexDouble)));
                if (!series[index].HasValue)
                    return index;

                var prevIndex = Math.Max(0, index - 1);
                if (!series[prevIndex].HasValue)
                {
                    series[prevIndex] = series[index];
                    return index;
                }

                var nextIndex = Math.Min(series.Count - 1, index + 1);
                if ((indexDouble - index) >= 0.5d && !series[nextIndex].HasValue)
                    return nextIndex;

                return index;
            }

            series[findBestIndex()] = valueToMap.Value;
        }

        private Dictionary<Tuple<ModuleId, ModuleId, Measure>, List<TimeSeries<double>>> ReadAndSaveMidresSensorValues(Dictionary<Tuple<ModuleId, ModuleId, Measure>, List<TimeSeries<double>>> loadedValues)
        {
            var readSeries = new Dictionary<Tuple<ModuleId, ModuleId, Measure>, List<TimeSeries<double>>>();

            foreach (var loaded in loadedValues)
            {
                var readList = new List<TimeSeries<double>>();
                foreach (var timeseries in loaded.Value)
                {
                    var dbSensorSeries = GetOrCreateEntity(_dbContext.NetatmoMeasureDataSet, timeseries.Begin, loaded.Key.Item2, loaded.Key.Item3);

                    var series = dbSensorSeries.MeasureSeries;
                    foreach (var item in timeseries)
                        if (item.Value.HasValue)
                            series[item.Key] = item.Value.Value;
                    dbSensorSeries.SetSeries(0, series);
                    readList.Add(series);
                }
                readSeries.Add(loaded.Key, readList);
            }

            return readSeries;
        }

        private void SaveLowresSensorValues(Dictionary<Tuple<ModuleId, ModuleId, Measure>, List<TimeSeries<double>>> midresSeries)
        {
            foreach (var loaded in midresSeries)
            {
                foreach (var timeseries in loaded.Value)
                {
                    DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
                    var dbSensorSeries = GetOrCreateEntity(_dbContext.NetatmoMeasureLowresDataSet, FirstOfMonth(timeseries.Begin), loaded.Key.Item2, loaded.Key.Item3);

                    var seriesToWriteInto = dbSensorSeries.MeasureSeries;
                    var resampler = new TimeSeriesResampler<TimeSeries<double>, double>(seriesToWriteInto.Span)
                    {
                        Resampled = seriesToWriteInto
                    };
                    resampler.SampleAggregate(timeseries, x => x.Average());

                    dbSensorSeries.SetSeries(0, resampler.Resampled);
                }
            }
        }

        private T GetOrCreateEntity<T>(DbSet<T> set, DateTime key, ModuleId module, Measure measure) where T : NetatmoMeasureData
        {
            string moduleId = module, measureKey = measure.ToString();
            var dbSensorSeries = set.Include(x => x.ModuleMeasure)
                .Where(x => x.ModuleMeasure.ModuleId == moduleId && x.ModuleMeasure.Measure == measureKey)
                .Where(x => x.Key == key).FirstOrDefault();
            if (dbSensorSeries != null)
                return dbSensorSeries;

            var dbModuleMeasure = _dbContext.NetatmoModuleMeasures.Where(x => x.ModuleId == moduleId && x.Measure == measureKey).FirstOrDefault();
            if (dbModuleMeasure == null)
            {
                var deviceId = _netatmoStructure.Modules.Where(x => x.Item2 == module).FirstOrDefault().Item1;
                dbModuleMeasure = new NetatmoModuleMeasure()
                {
                    DeviceId = deviceId,
                    ModuleId = moduleId,
                    Measure = measureKey,
                    StationName = _netatmoStructure.GetDeviceName(deviceId, 30),
                    ModuleName = _netatmoStructure.GetModuleName(module, 30)
                };
                dbModuleMeasure.SetDecimalsByMeasureType(measure);
                _dbContext.NetatmoModuleMeasures.Add(dbModuleMeasure);
            }

            dbSensorSeries = Activator.CreateInstance<T>();
            dbSensorSeries.ModuleMeasureId = dbModuleMeasure.Id;
            dbSensorSeries.ModuleMeasure = dbModuleMeasure;
            dbSensorSeries.Key = key;
            dbSensorSeries.Decimals = dbModuleMeasure.Decimals;
            set.Add(dbSensorSeries);
            return dbSensorSeries;
        }
    }
}