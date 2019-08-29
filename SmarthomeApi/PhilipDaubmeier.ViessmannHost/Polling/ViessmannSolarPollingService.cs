using Microsoft.Extensions.Logging;
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
                _dbContext.ViessmannSolarTimeseries.Add(dbSolarSeries = new ViessmannSolarData() { Key = day });

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

            _dbContext.SaveChanges();
        }
    }
}