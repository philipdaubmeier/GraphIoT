using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Core
{
    public class TimedHostedPollingService<TPollingService> : IHostedService, IDisposable where TPollingService : IScopedPollingService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;
        private readonly TimeSpan _interval;
        private readonly string _loggingName;
        private Timer _timer;

        public TimedHostedPollingService(IServiceProvider services, ILogger<TimedHostedPollingService<TPollingService>> logger, IOptions<TimedHostedPollingConfig<TPollingService>> config)
        {
            _services = services;
            _logger = logger;
            _interval = config?.Value?.TimerIntervalTimeSpan ?? TimeSpan.FromMinutes(1);
            _loggingName = config?.Value?.Name ?? string.Empty;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} {_loggingName} Background Service is starting.");

            _timer = new Timer(PollAll, null, TimeSpan.Zero, _interval);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} {_loggingName} Background Service is stopping.");

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
                foreach (var service in scope.ServiceProvider.GetServices<TPollingService>())
                    await service.PollValues();
            }
        }
    }
}