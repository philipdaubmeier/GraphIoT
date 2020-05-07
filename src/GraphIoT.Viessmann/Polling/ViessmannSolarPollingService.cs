using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
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

            var solarWhTotal = int.Parse(data.First(d => d.id == 7895.ToString()).value);
            var solarCollectorTemp = double.Parse(data.First(d => d.id == 5272.ToString()).value, CultureInfo.InvariantCulture);
            var solarHotwaterTemp = double.Parse(data.First(d => d.id == 5276.ToString()).value, CultureInfo.InvariantCulture);
            var solarPumpState = data.First(d => d.id == 5274.ToString()).value.Trim() != "0";
            var solarSuppression = data.First(d => d.id == 5273.ToString()).value.Trim() != "0";

            SaveSolarValues(_dbContext, time, solarWhTotal, solarCollectorTemp, solarHotwaterTemp, solarPumpState, solarSuppression);
        }

        public static void SaveSolarValues(IViessmannDbContext dbContext, DateTime time, int solarWhTotal, double solarCollectorTemp, double solarHotwaterTemp, bool solarPumpState, bool solarSuppression)
        {
            var dbSolarSeries = TimeSeriesDbEntityBase.LoadOrCreateDay(dbContext.ViessmannSolarTimeseries, time.Date);

            var oldSolarWhTotal = dbSolarSeries.SolarWhTotal;
            var series1 = dbSolarSeries.SolarWhSeries;
            series1.Accumulate(time, oldSolarWhTotal.HasValue ? solarWhTotal - oldSolarWhTotal.Value : 0);
            dbSolarSeries.SetSeries(0, series1);
            dbSolarSeries.SolarWhTotal = solarWhTotal;

            dbSolarSeries.SetSeriesValue(1, time, solarCollectorTemp);
            dbSolarSeries.SetSeriesValue(2, time, solarHotwaterTemp);
            dbSolarSeries.SetSeriesValue(3, time, solarPumpState);
            dbSolarSeries.SetSeriesValue(4, time, solarSuppression);

            SaveLowresSolarValues(dbContext, time.Date, dbSolarSeries);

            dbContext.SaveChanges();
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