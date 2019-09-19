using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.ViessmannClient;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Polling
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

            var time = DateTime.UtcNow;
            var day = time.Date;
            
            var dbHeatingSeries = _dbContext.ViessmannHeatingTimeseries.Where(x => x.Key == day).FirstOrDefault();
            if (dbHeatingSeries == null)
                _dbContext.ViessmannHeatingTimeseries.Add(dbHeatingSeries = new ViessmannHeatingMidresData() { Key = day, BurnerHoursTotal = 0d, BurnerStartsTotal = 0 });

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

            var series3 = dbHeatingSeries.BurnerModulationSeries;
            series3[time] = burnerModulation;
            dbHeatingSeries.SetSeries(2, series3);

            var series4 = dbHeatingSeries.OutsideTempSeries;
            series4[time] = outsideTemp;
            dbHeatingSeries.SetSeries(3, series4);

            var series5 = dbHeatingSeries.BoilerTempSeries;
            series5[time] = boilerTemp;
            dbHeatingSeries.SetSeries(4, series5);

            var series6 = dbHeatingSeries.BoilerTempMainSeries;
            series6[time] = boilerTempMain;
            dbHeatingSeries.SetSeries(5, series6);

            var series7 = dbHeatingSeries.Circuit0TempSeries;
            series7[time] = circuit0Temp;
            dbHeatingSeries.SetSeries(6, series7);

            var series8 = dbHeatingSeries.Circuit1TempSeries;
            series8[time] = circuit1Temp;
            dbHeatingSeries.SetSeries(7, series8);

            var series9 = dbHeatingSeries.DhwTempSeries;
            series9[time] = dhwTemp;
            dbHeatingSeries.SetSeries(8, series9);

            var series10 = dbHeatingSeries.BurnerActiveSeries;
            series10[time] = burnerActive;
            dbHeatingSeries.SetSeries(8, series10);

            var series11 = dbHeatingSeries.Circuit0PumpSeries;
            series11[time] = circuit0Pump;
            dbHeatingSeries.SetSeries(10, series11);

            var series12 = dbHeatingSeries.Circuit1PumpSeries;
            series12[time] = circuit1Pump;
            dbHeatingSeries.SetSeries(11, series12);

            var series13 = dbHeatingSeries.DhwPrimaryPumpSeries;
            series13[time] = dhwPrimPump;
            dbHeatingSeries.SetSeries(12, series13);

            var series14 = dbHeatingSeries.DhwCirculationPumpSeries;
            series14[time] = dhwCircPump;
            dbHeatingSeries.SetSeries(13, series14);

            SaveLowresHeatingValues(day, series1, series2, series3, series4, series5, series6, series7, series8, series9, series10, series11, series12, series13, series14);

            _dbContext.SaveChanges();
        }

        public void GenerateLowResHeatingSeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                var dbHeatingSeries = _dbContext.ViessmannHeatingTimeseries.Where(x => x.Key == day).FirstOrDefault();
                if (dbHeatingSeries == null)
                    continue;

                SaveLowresHeatingValues(day, dbHeatingSeries.BurnerMinutesSeries, dbHeatingSeries.BurnerStartsSeries, dbHeatingSeries.BurnerModulationSeries, dbHeatingSeries.OutsideTempSeries,
                    dbHeatingSeries.BoilerTempSeries, dbHeatingSeries.BoilerTempMainSeries, dbHeatingSeries.Circuit0TempSeries, dbHeatingSeries.Circuit1TempSeries, dbHeatingSeries.DhwTempSeries,
                    dbHeatingSeries.BurnerActiveSeries, dbHeatingSeries.Circuit0PumpSeries, dbHeatingSeries.Circuit1PumpSeries, dbHeatingSeries.DhwPrimaryPumpSeries, dbHeatingSeries.DhwCirculationPumpSeries);
                _dbContext.SaveChanges();
            }
        }

        private void SaveLowresHeatingValues(DateTime day, TimeSeries<double> series1Src, TimeSeries<int> series2Src, TimeSeries<int> series3Src, TimeSeries<double> series4Src,
            TimeSeries<double> series5Src, TimeSeries<double> series6Src, TimeSeries<double> series7Src, TimeSeries<double> series8Src, TimeSeries<double> series9Src,
            TimeSeries<bool> series10Src, TimeSeries<bool> series11Src, TimeSeries<bool> series12Src, TimeSeries<bool> series13Src, TimeSeries<bool> series14Src)
        {
            DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
            var month = FirstOfMonth(day);
            var dbHeatingSeries = _dbContext.ViessmannHeatingLowresTimeseries.Where(x => x.Key == month).FirstOrDefault();
            if (dbHeatingSeries == null)
                _dbContext.ViessmannHeatingLowresTimeseries.Add(dbHeatingSeries = new ViessmannHeatingLowresData() { Key = month });

            var series1 = dbHeatingSeries.BurnerMinutesSeries;
            var resampler1 = new TimeSeriesResampler<TimeSeries<double>, double>(series1.Span) { Resampled = series1 };
            resampler1.SampleAggregate(series1Src, x => x.Average());
            dbHeatingSeries.SetSeries(0, resampler1.Resampled);

            var series2 = dbHeatingSeries.BurnerStartsSeries;
            var resampler2 = new TimeSeriesResampler<TimeSeries<int>, int>(series2.Span) { Resampled = series2 };
            resampler2.SampleAggregate(series2Src, x => (int)x.Average());
            dbHeatingSeries.SetSeries(1, resampler2.Resampled);

            var series3 = dbHeatingSeries.BurnerModulationSeries;
            var resampler3 = new TimeSeriesResampler<TimeSeries<int>, int>(series3.Span) { Resampled = series3 };
            resampler3.SampleAggregate(series3Src, x => (int)x.Average());
            dbHeatingSeries.SetSeries(2, resampler3.Resampled);

            var series4 = dbHeatingSeries.OutsideTempSeries;
            var resampler4 = new TimeSeriesResampler<TimeSeries<double>, double>(series4.Span) { Resampled = series4 };
            resampler4.SampleAggregate(series4Src, x => x.Average());
            dbHeatingSeries.SetSeries(3, resampler4.Resampled);

            var series5 = dbHeatingSeries.BoilerTempSeries;
            var resampler5 = new TimeSeriesResampler<TimeSeries<double>, double>(series5.Span) { Resampled = series5 };
            resampler5.SampleAggregate(series5Src, x => x.Average());
            dbHeatingSeries.SetSeries(4, resampler5.Resampled);

            var series6 = dbHeatingSeries.BoilerTempMainSeries;
            var resampler6 = new TimeSeriesResampler<TimeSeries<double>, double>(series6.Span) { Resampled = series6 };
            resampler6.SampleAggregate(series6Src, x => x.Average());
            dbHeatingSeries.SetSeries(5, resampler6.Resampled);

            var series7 = dbHeatingSeries.Circuit0TempSeries;
            var resampler7 = new TimeSeriesResampler<TimeSeries<double>, double>(series7.Span) { Resampled = series7 };
            resampler7.SampleAggregate(series7Src, x => x.Average());
            dbHeatingSeries.SetSeries(6, resampler7.Resampled);

            var series8 = dbHeatingSeries.Circuit1TempSeries;
            var resampler8 = new TimeSeriesResampler<TimeSeries<double>, double>(series8.Span) { Resampled = series8 };
            resampler8.SampleAggregate(series8Src, x => x.Average());
            dbHeatingSeries.SetSeries(7, resampler8.Resampled);

            var series9 = dbHeatingSeries.DhwTempSeries;
            var resampler9 = new TimeSeriesResampler<TimeSeries<double>, double>(series9.Span) { Resampled = series9 };
            resampler9.SampleAggregate(series9Src, x => x.Average());
            dbHeatingSeries.SetSeries(8, resampler9.Resampled);

            var series10 = dbHeatingSeries.BurnerActiveSeries;
            var resampler10 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series10.Span) { Resampled = series10 };
            resampler10.SampleAggregate(series10Src, x => x.Any(v => v));
            dbHeatingSeries.SetSeries(9, resampler10.Resampled);

            var series11 = dbHeatingSeries.Circuit0PumpSeries;
            var resampler11 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series11.Span) { Resampled = series11 };
            resampler11.SampleAggregate(series11Src, x => x.Any(v => v));
            dbHeatingSeries.SetSeries(10, resampler11.Resampled);

            var series12 = dbHeatingSeries.Circuit1PumpSeries;
            var resampler12 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series12.Span) { Resampled = series12 };
            resampler12.SampleAggregate(series12Src, x => x.Any(v => v));
            dbHeatingSeries.SetSeries(11, resampler12.Resampled);

            var series13 = dbHeatingSeries.DhwPrimaryPumpSeries;
            var resampler13 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series13.Span) { Resampled = series13 };
            resampler13.SampleAggregate(series13Src, x => x.Any(v => v));
            dbHeatingSeries.SetSeries(12, resampler13.Resampled);

            var series14 = dbHeatingSeries.DhwCirculationPumpSeries;
            var resampler14 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series14.Span) { Resampled = series14 };
            resampler14.SampleAggregate(series14Src, x => x.Any(v => v));
            dbHeatingSeries.SetSeries(13, resampler14.Resampled);
        }
    }
}