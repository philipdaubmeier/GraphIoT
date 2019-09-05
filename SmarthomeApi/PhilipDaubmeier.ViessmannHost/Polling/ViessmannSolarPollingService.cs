using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.ViessmannClient;
using PhilipDaubmeier.ViessmannHost.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannHost.Polling
{
    public class ViessmannSolarPollingService : IViessmannPollingService
    {
        private readonly ILogger _logger;
        private readonly IViessmannDbContext _dbContext;
        private readonly ViessmannVitotrolClient _vitotrolClient;

        public ViessmannSolarPollingService(ILogger<ViessmannSolarPollingService> logger, IViessmannDbContext dbContext, ViessmannVitotrolClient vitotrolClient)
        {
            _logger = logger;
            _dbContext = dbContext;
            _vitotrolClient = vitotrolClient;
        }
        
        public async Task PollValues()
        {
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
        
        private async Task PollSolarValues()
        {
            var data = await _vitotrolClient.GetData(new List<int>() { 5272, 5273, 5274, 5276, 7895 });
            var time = data.First().Item3;
            var day = time.Date;
            
            var dbSolarSeries = _dbContext.ViessmannSolarTimeseries.Where(x => x.Key == day).FirstOrDefault();
            if (dbSolarSeries == null)
                _dbContext.ViessmannSolarTimeseries.Add(dbSolarSeries = new ViessmannSolarMidresData() { Key = day });

            var item = data.First(d => d.Item1 == 7895.ToString());
            var newValue = int.Parse(item.Item2);
            var oldValue = dbSolarSeries.SolarWhTotal ?? 0;
            var series1 = dbSolarSeries.SolarWhSeries;
            series1.Accumulate(time, newValue - oldValue);
            dbSolarSeries.SetSeries(0, series1);
            dbSolarSeries.SolarWhTotal = newValue;

            item = data.First(d => d.Item1 == 5272.ToString());
            var series2 = dbSolarSeries.SolarCollectorTempSeries;
            series2[time] = double.Parse(item.Item2, CultureInfo.InvariantCulture);
            dbSolarSeries.SetSeries(1, series2);

            item = data.First(d => d.Item1 == 5276.ToString());
            var series3 = dbSolarSeries.SolarHotwaterTempSeries;
            series3[time] = double.Parse(item.Item2, CultureInfo.InvariantCulture);
            dbSolarSeries.SetSeries(2, series3);

            item = data.First(d => d.Item1 == 5274.ToString());
            var series4 = dbSolarSeries.SolarPumpStateSeries;
            series4[time] = item.Item2.Trim() != "0";
            dbSolarSeries.SetSeries(3, series4);

            item = data.First(d => d.Item1 == 5273.ToString());
            var series5 = dbSolarSeries.SolarSuppressionSeries;
            series5[time] = item.Item2.Trim() != "0";
            dbSolarSeries.SetSeries(4, series5);

            SaveLowresSolarValues(day, series1, series2, series3, series4, series5);

            _dbContext.SaveChanges();
        }

        public void GenerateLowResSolarSeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                var dbSolarSeries = _dbContext.ViessmannSolarTimeseries.Where(x => x.Key == day).FirstOrDefault();
                if (dbSolarSeries == null)
                    continue;

                SaveLowresSolarValues(day, dbSolarSeries.SolarWhSeries, dbSolarSeries.SolarCollectorTempSeries, dbSolarSeries.SolarHotwaterTempSeries, dbSolarSeries.SolarPumpStateSeries, dbSolarSeries.SolarSuppressionSeries);
                _dbContext.SaveChanges();
            }
        }

        private void SaveLowresSolarValues(DateTime day, TimeSeries<int> series1Src, TimeSeries<double> series2Src, TimeSeries<double> series3Src, TimeSeries<bool> series4Src, TimeSeries<bool> series5Src)
        {
            DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
            var month = FirstOfMonth(day);
            var dbSolarSeries = _dbContext.ViessmannSolarLowresTimeseries.Where(x => x.Key == month).FirstOrDefault();
            if (dbSolarSeries == null)
                _dbContext.ViessmannSolarLowresTimeseries.Add(dbSolarSeries = new ViessmannSolarLowresData() { Key = month });

            // Hack: remove first 5 elements due to bug in day-boundaries
            ITimeSeries<int> PreprocessSolarProduction(ITimeSeries<int> input)
            {
                for (int i = 0; i < 5; i++)
                    input[i] = input[i].HasValue ? (int?)0 : null;
                return input;
            }

            var series1 = dbSolarSeries.SolarWhSeries;
            var resampler1 = new TimeSeriesResampler<TimeSeries<int>, int>(series1.Span) { Resampled = series1 };
            resampler1.SampleAggregate(PreprocessSolarProduction(series1Src), x => (int)x.Average());
            dbSolarSeries.SetSeries(0, resampler1.Resampled);

            var series2 = dbSolarSeries.SolarCollectorTempSeries;
            var resampler2 = new TimeSeriesResampler<TimeSeries<double>, double>(series2.Span) { Resampled = series2 };
            resampler2.SampleAggregate(series2Src, x => x.Average());
            dbSolarSeries.SetSeries(1, resampler2.Resampled);

            var series3 = dbSolarSeries.SolarHotwaterTempSeries;
            var resampler3 = new TimeSeriesResampler<TimeSeries<double>, double>(series3.Span) { Resampled = series3 };
            resampler3.SampleAggregate(series3Src, x => x.Average());
            dbSolarSeries.SetSeries(2, resampler3.Resampled);

            var series4 = dbSolarSeries.SolarPumpStateSeries;
            var resampler4 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series4.Span) { Resampled = series4 };
            resampler4.SampleAggregate(series4Src, x => x.Any(v => v));
            dbSolarSeries.SetSeries(3, resampler4.Resampled);

            var series5 = dbSolarSeries.SolarSuppressionSeries;
            var resampler5 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series5.Span) { Resampled = series5 };
            resampler5.SampleAggregate(series5Src, x => x.Any(v => v));
            dbSolarSeries.SetSeries(4, resampler5.Resampled);
        }
    }
}