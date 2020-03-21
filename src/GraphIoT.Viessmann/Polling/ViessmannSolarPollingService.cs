using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using PhilipDaubmeier.ViessmannClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Polling
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
            var time = data.First().timestamp.ToUniversalTime();

            var item = data.First(d => d.id == 7895.ToString());
            var solarWhTotal = int.Parse(item.value);

            item = data.First(d => d.id == 5272.ToString());
            var solarCollectorTemp = double.Parse(item.value, CultureInfo.InvariantCulture);

            item = data.First(d => d.id == 5276.ToString());
            var solarHotwaterTemp = double.Parse(item.value, CultureInfo.InvariantCulture);

            item = data.First(d => d.id == 5274.ToString());
            var solarPumpState = item.value.Trim() != "0";

            item = data.First(d => d.id == 5273.ToString());
            var solarSuppression = item.value.Trim() != "0";

            SaveSolarValues(_dbContext, time, solarWhTotal, solarCollectorTemp, solarHotwaterTemp, solarPumpState, solarSuppression);
        }

        public static void SaveSolarValues(IViessmannDbContext dbContext, DateTime time, int solarWhTotal, double solarCollectorTemp, double solarHotwaterTemp, bool solarPumpState, bool solarSuppression)
        {
            var day = time.Date;

            var dbSolarSeries = dbContext.ViessmannSolarTimeseries.Where(x => x.Key == day).FirstOrDefault();
            if (dbSolarSeries == null)
                dbContext.ViessmannSolarTimeseries.Add(dbSolarSeries = new ViessmannSolarMidresData() { Key = day });

            var oldSolarWhTotal = dbSolarSeries.SolarWhTotal;
            var series1 = dbSolarSeries.SolarWhSeries;
            series1.Accumulate(time, oldSolarWhTotal.HasValue ? solarWhTotal - oldSolarWhTotal.Value : 0);
            dbSolarSeries.SetSeries(0, series1);
            dbSolarSeries.SolarWhTotal = solarWhTotal;

            var series2 = dbSolarSeries.SolarCollectorTempSeries;
            series2[time] = solarCollectorTemp;
            dbSolarSeries.SetSeries(1, series2);

            var series3 = dbSolarSeries.SolarHotwaterTempSeries;
            series3[time] = solarHotwaterTemp;
            dbSolarSeries.SetSeries(2, series3);

            var series4 = dbSolarSeries.SolarPumpStateSeries;
            series4[time] = solarPumpState;
            dbSolarSeries.SetSeries(3, series4);

            var series5 = dbSolarSeries.SolarSuppressionSeries;
            series5[time] = solarSuppression;
            dbSolarSeries.SetSeries(4, series5);

            SaveLowresSolarValues(dbContext, day, series1, series2, series3, series4, series5);

            dbContext.SaveChanges();
        }

        public void GenerateLowResSolarSeries(DateTime start, DateTime end)
        {
            foreach (var day in new TimeSeriesSpan(start, end, 1).IncludedDates())
            {
                var dbSolarSeries = _dbContext.ViessmannSolarTimeseries.Where(x => x.Key == day).FirstOrDefault();
                if (dbSolarSeries == null)
                    continue;

                SaveLowresSolarValues(_dbContext, day, dbSolarSeries.SolarWhSeries, dbSolarSeries.SolarCollectorTempSeries, dbSolarSeries.SolarHotwaterTempSeries, dbSolarSeries.SolarPumpStateSeries, dbSolarSeries.SolarSuppressionSeries);
                _dbContext.SaveChanges();
            }
        }

        private static void SaveLowresSolarValues(IViessmannDbContext dbContext, DateTime day, TimeSeries<int> series1Src, TimeSeries<double> series2Src, TimeSeries<double> series3Src, TimeSeries<bool> series4Src, TimeSeries<bool> series5Src)
        {
            DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
            var month = FirstOfMonth(day);
            var dbSolarSeries = dbContext.ViessmannSolarLowresTimeseries.Where(x => x.Key == month).FirstOrDefault();
            if (dbSolarSeries == null)
                dbContext.ViessmannSolarLowresTimeseries.Add(dbSolarSeries = new ViessmannSolarLowresData() { Key = month });

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