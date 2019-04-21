using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Services
{
    public class DigitalstromTimedHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;
        private Timer _timer;

        public DigitalstromTimedHostedService(IServiceProvider services, ILogger<DigitalstromTimedHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is starting.");

            _timer = new Timer(PollAll, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async void PollAll(object state)
        {
            using (var scope = _services.CreateScope())
            {
                await PollValuesScoped<DigitalstromEnergyPollingService>(scope);
                await PollValuesScoped<DigitalstromSensorPollingService>(scope);
            }
        }

        private async Task PollValuesScoped<T>(IServiceScope scope) where T : IScopedPollingService
        {
            await scope.ServiceProvider.GetRequiredService<T>().PollValues();
        }
    }
}