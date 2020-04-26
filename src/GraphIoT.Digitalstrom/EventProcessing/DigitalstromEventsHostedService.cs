using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromTwin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.EventProcessing
{
    public class DigitalstromEventsHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<DigitalstromEventProcessingConfig> _config;
        private readonly IDigitalstromConnectionProvider _connProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

#pragma warning disable IDE0069 // false warning - is properly disposed
        private DssEventSubscriber? _dssEventSubscriber = null;
#pragma warning restore IDE0069

        private Task? _executingThread;
        private CancellationTokenSource? _cancellationSource;
        private CancellationToken _cancellationToken;
        private BlockingCollection<DssEvent>? _persistenceQueue;

        private readonly IEnumerable<IDigitalstromEventProcessorPlugin> _plugins;

        public DigitalstromEventsHostedService(ILogger<DigitalstromEventsHostedService> logger, IOptions<DigitalstromEventProcessingConfig> config, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _config = config;
            _serviceScopeFactory = serviceScopeFactory;
            _serviceScope = _serviceScopeFactory.CreateScope();
            _connProvider = _serviceScope.ServiceProvider.GetRequiredService<IDigitalstromConnectionProvider>();
            _plugins = _serviceScope.ServiceProvider.GetServices<IDigitalstromEventProcessorPlugin>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_dssEventSubscriber != null)
                throw new InvalidOperationException("Digitalstrom Event Subscriber Service is already started");

            _logger.LogInformation($"{DateTime.Now} Digitalstrom Event Subscriber Service is starting.");

            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            _persistenceQueue = new BlockingCollection<DssEvent>();
            _executingThread = Task.Factory.StartNew(() => PersistenceWorkerThread(), _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _dssEventSubscriber = new DssEventSubscriber(_connProvider, _plugins.SelectMany(p => p.EventNames));
            _dssEventSubscriber.ApiEventRaised += (s, e) => DigitalstromEventReceived(e.ApiEvent);
            _dssEventSubscriber.ErrorOccured += (s, e) => DigitalstromSceneClientError(e.Error);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingThread is null)
                return;

            _logger.LogInformation($"{DateTime.Now} Digitalstrom Event Subscriber Service is stopping.");

            _dssEventSubscriber?.Dispose();
            _dssEventSubscriber = null;

            _persistenceQueue?.CompleteAdding();
            _persistenceQueue = null;

            try
            {
                // Signal cancellation to the executing method
                _cancellationSource?.Cancel();
            }
            finally
            {
                // Wait until the thread completes or the stop token triggers
                await Task.WhenAny(_executingThread, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            // Disposing the event subscriber stops all its onderlying threads
            _dssEventSubscriber?.Dispose();

            _cancellationSource?.Cancel();
            _cancellationSource?.Dispose();
            _persistenceQueue?.CompleteAdding();
            _persistenceQueue?.Dispose();

            // DI Scope ends here
            _serviceScope.Dispose();
        }

        private void DigitalstromEventReceived(DssEvent dsEvent)
        {
            _logger.LogInformation($"{DateTime.Now} Received DSS Event: name '{dsEvent.Name}' zone '{dsEvent.Properties.ZoneID}' group '{dsEvent.Properties.GroupID}' scene '{dsEvent.Properties.SceneID}'");

            if (!(_persistenceQueue?.TryAdd(dsEvent) ?? false))
                _logger.LogError($"{DateTime.Now} Exception in Digitalstrom Event Subscriber Service: could not enqueue dss event!");
        }

        private void DigitalstromSceneClientError(Exception ex)
        {
            _logger.LogError($"{DateTime.Now} Exception in Digitalstrom Event Subscriber Service in event polling thread", ex);
        }

        /// <summary>
        /// Digitalstrom events are often "clumped up", i.e. time duration between events is on the one hand
        /// relatively large and then again after the first occurence there likely follow a few next ones in
        /// quite short succession. To accomodate for this and to reduce database write load, this worker
        /// thread loop consumes from the blocking queue of DssEvents and as soon as it receives one, it
        /// tries to collect all next ones for 'collectItemsMs' milliseconds long. Only then it writes all
        /// received events to the database. Put another way, this ensures that in the worst case, only all
        /// 'collectItemsMs' milliseconds a database write is triggered.
        /// </summary>
        private void PersistenceWorkerThread()
        {
            Thread.CurrentThread.Name = "Digitalstrom Scene Event Persistence Thread";

            int collectItemsMs = (int)_config.Value.ItemCollectionTimeSpan.TotalMilliseconds;

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    DequeueAndPipeToEventStream(-1);

                    var dateFirstReceived = DateTime.UtcNow;
                    while (DateTime.UtcNow < dateFirstReceived.AddMilliseconds(collectItemsMs) && !_cancellationToken.IsCancellationRequested)
                    {
                        var remainingMs = (int)(dateFirstReceived.AddMilliseconds(collectItemsMs) - DateTime.UtcNow).TotalMilliseconds;
                        DequeueAndPipeToEventStream(remainingMs);
                    }

                    foreach (var plugin in _plugins)
                        plugin.SaveEventStreamToDb();

                    _logger.LogInformation($"{DateTime.Now} Persisted event clump");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{DateTime.Now} Exception in Digitalstrom Event Subscriber Service in persistence thread", ex);
                }
            }
        }

        /// <summary>
        /// Blocks for the given millisecondsTimeout or until the next item is in the queue and dequeues the
        /// DssEvent. The event is then written to the event stream, which is read from the db or created if
        /// not existing. If a new day is reached, it automatically creates a new event stream which it writes to.
        /// </summary>
        private void DequeueAndPipeToEventStream(int millisecondsTimeout)
        {
            if (_persistenceQueue is null)
                return;
            _persistenceQueue.TryTake(out DssEvent? dsEvent, millisecondsTimeout, _cancellationToken);

            // Timeout was triggered, no event was read - leave method and continue in main thread loop
            if (dsEvent is null)
                return;

            if (millisecondsTimeout < 0)
                _logger.LogInformation($"{DateTime.Now} Dequeued event (first in clump) with timestamp {dsEvent.TimestampUtc}");
            else
                _logger.LogInformation($"{DateTime.Now} Dequeued event (clump successor) with timestamp {dsEvent.TimestampUtc}");

            foreach (var plugin in _plugins.Where(p => p.EventNames.Contains(dsEvent.SystemEvent)))
            {
                plugin.ReadOrCreateEventStream(dsEvent.TimestampUtc.ToLocalTime().Date);

                // We have already written the same event the last (configurable) interval or sooner
                if (plugin.HasDuplicate(dsEvent, (int)_config.Value.DuplicateDetectionTimeSpan.TotalMilliseconds))
                    continue;

                plugin.WriteToEventStream(dsEvent);
            }
        }
    }
}