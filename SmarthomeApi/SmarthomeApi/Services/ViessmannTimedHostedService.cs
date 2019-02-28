using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmarthomeApi.Clients.Viessmann;
using SmarthomeApi.Database.Model;

namespace SmarthomeApi.Services
{
    public class ViessmannTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly PersistenceContext _dbContext;
        private readonly ViessmannVitotrolClient _vitotrolClient;
        private Timer _timer;

        public ViessmannTimedHostedService(ILogger<ViessmannTimedHostedService> logger, PersistenceContext databaseContext)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _vitotrolClient = new ViessmannVitotrolClient(_dbContext);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Viessmann Background Service is starting.");

            _timer = new Timer(PollSolarValues, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Viessmann Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async void PollSolarValues(object state)
        {
            _logger.LogInformation($"{DateTime.Now} Viessmann Background Service is polling new values...");

            try
            {
                // 7895 Solarertrag in Wh
                // 5272 Kollektortemperatur in °C
                // 5276 Warmwasser Solar in °C
                // 5274 Solarkreispumpe 0=aus 1=ein
                // 5273 Nachheizunterdrück. 0=aus 1=ein
                var data = await _vitotrolClient.GetData(new List<int>() { 5272, 5273, 5274, 5276, 7895 });
                _logger.LogInformation($"{DateTime.Now} New value {data.First().Item2} at {data.First().Item3}");

                var time = data.First().Item3;
                var day = time.Date;
                var dbSolarSeries = _dbContext.ViessmannSolarTimeseries.Where(x => x.Day == day).FirstOrDefault();
                if (dbSolarSeries == null)
                    _dbContext.ViessmannSolarTimeseries.Add(dbSolarSeries = new ViessmannSolarData() { Day = day });

                var item = data.First(d => d.Item1 == 7895.ToString());
                var newValue = int.Parse(item.Item2);
                var oldValue = dbSolarSeries.SolarWhTotal.HasValue ? dbSolarSeries.SolarWhTotal.Value : 0;
                var series1 = dbSolarSeries.SolarWhSeries;
                series1.Accumulate(time, newValue - oldValue);
                dbSolarSeries.SolarWhSeries = series1;
                dbSolarSeries.SolarWhTotal = newValue;

                item = data.First(d => d.Item1 == 5272.ToString());
                var series2 = dbSolarSeries.SolarCollectorTempSeries;
                series2[time] = double.Parse(item.Item2, CultureInfo.InvariantCulture);
                dbSolarSeries.SolarCollectorTempSeries = series2;

                item = data.First(d => d.Item1 == 5276.ToString());
                var series3 = dbSolarSeries.SolarHotwaterTempSeries;
                series3[time] = double.Parse(item.Item2, CultureInfo.InvariantCulture);
                dbSolarSeries.SolarHotwaterTempSeries = series3;

                item = data.First(d => d.Item1 == 5274.ToString());
                var series4 = dbSolarSeries.SolarPumpStateSeries;
                series4[time] = item.Item2.Trim() != "0";
                dbSolarSeries.SolarPumpStateSeries = series4;

                item = data.First(d => d.Item1 == 5273.ToString());
                var series5 = dbSolarSeries.SolarSuppressionSeries;
                series5[time] = item.Item2.Trim() != "0";
                dbSolarSeries.SolarSuppressionSeries = series5;

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in viessmann background worker: {ex.Message}");
            }
        }
    }
}
