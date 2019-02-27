using System;
using System.Collections.Generic;
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

            var data = await _vitotrolClient.GetData(new List<int>() { 7895 });
            _logger.LogInformation($"{DateTime.Now} New value {data.First().Item2} at {data.First().Item3}");
        }
    }
}
