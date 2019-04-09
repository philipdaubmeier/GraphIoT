using CompactTimeSeries;
using DigitalstromClient.Model;
using DigitalstromClient.Model.Events;
using DigitalstromClient.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmarthomeApi.Database.Model;
using SmarthomeApi.Model;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmarthomeApi.Services
{
    public class DigitalstromEventsHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly PersistenceContext _dbContext;
        private readonly IDigitalstromConnectionProvider _connProvider;
        private DigitalstromSceneClient _dsSceneClient = null;

        private CancellationTokenSource _cancellationSource;
        private CancellationToken _cancellationToken;
        private BlockingCollection<DssEvent> _persistenceQueue;
        private Task _persistenceWorkerThread = null;
        
        private SceneEventStream _eventStream = null;

        public DigitalstromEventsHostedService(ILogger<DigitalstromEventsHostedService> logger, PersistenceContext databaseContext, IDigitalstromConnectionProvider connectionProvider)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _connProvider = connectionProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_dsSceneClient != null)
                throw new InvalidOperationException("Digitalstrom Event Subscriber Service is already started");

            _logger.LogInformation($"{DateTime.Now} Digitalstrom Event Subscriber Service is starting.");

            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            _persistenceQueue = new BlockingCollection<DssEvent>();
            _persistenceWorkerThread = Task.Factory.StartNew(() => PersistenceWorkerThread(), _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _dsSceneClient = new DigitalstromSceneClient(_connProvider);
            _dsSceneClient.ApiEventRaised += (s, e) => DigitalstromEventReceived(e.ApiEvent);
            _dsSceneClient.ErrorOccured += (s, e) => DigitalstromSceneClientError(e.Error);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Event Subscriber Service is stopping.");

            _dsSceneClient.Dispose();
            _dsSceneClient = null;

            _cancellationSource.Cancel();
            _persistenceQueue.CompleteAdding();
            _persistenceQueue = null;
            _persistenceWorkerThread = null;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // DigitalstromSceneClient.Dispose stops all its threads
            _dsSceneClient?.Dispose();

            _cancellationSource.Cancel();
            _persistenceQueue?.CompleteAdding();
        }

        private void DigitalstromEventReceived(DssEvent dsEvent)
        {
            _logger.LogInformation($"{DateTime.Now} Received DSS Event: name '{dsEvent.name}' zone '{dsEvent.properties.zone}' group '{dsEvent.properties.group}' scene '{dsEvent.properties.scene}'");

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

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    const int collectItemsMs = 5000;

                    DequeueAndPipeToEventStream(-1);

                    var dateFirstReceived = DateTime.UtcNow;
                    while (DateTime.UtcNow < dateFirstReceived.AddMilliseconds(collectItemsMs) && !_cancellationToken.IsCancellationRequested)
                    {
                        var remainingMs = (int)(dateFirstReceived.AddMilliseconds(collectItemsMs) - DateTime.UtcNow).TotalMilliseconds;
                        DequeueAndPipeToEventStream(remainingMs);
                    }

                    SaveEventStreamToDb();
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
        /// <param name="millisecondsTimeout"></param>
        private void DequeueAndPipeToEventStream(int millisecondsTimeout)
        {
            _persistenceQueue.TryTake(out DssEvent dsEvent, millisecondsTimeout, _cancellationToken);

            // Timeout was triggered, no event was read - leave method and continue in main thread loop
            if (dsEvent == null)
                return;

            if (millisecondsTimeout < 0)
                _logger.LogInformation($"{DateTime.Now} Dequeued event (first in clump) with timestamp {dsEvent?.TimestampUtc}");
            else
                _logger.LogInformation($"{DateTime.Now} Dequeued event (clump successor) with timestamp {dsEvent?.TimestampUtc}");

            // We try to save to the event stream the first time, so first create one
            if (_eventStream == null)
                ReadOrCreateEventStream(dsEvent.TimestampUtc.ToLocalTime().Date);

            // We have reached the day boundary - save the current event stream and create a new one for the new day
            if (!_eventStream.Span.IsIncluded(dsEvent.TimestampUtc.ToLocalTime()))
            {
                SaveEventStreamToDb();
                ReadOrCreateEventStream(dsEvent.TimestampUtc.ToLocalTime().Date);
            }

            _eventStream.WriteEvent(dsEvent);
        }

        /// <summary>
        /// Reads the event stream for the given day from the db or creates a new one if not existing yet.
        /// </summary>
        private void ReadOrCreateEventStream(DateTime date)
        {
            if (_eventStream != null && _eventStream.Span.IsIncluded(date))
                return;

            _dbContext.Semaphore.WaitOne();
            try
            {
                var dbSceneEvents = _dbContext.DsSceneEventDataSet.Where(x => x.Day == date).FirstOrDefault();
                if (dbSceneEvents != null)
                    _eventStream = dbSceneEvents.EventStream;
                else
                    _eventStream = new SceneEventStream(new TimeSeriesSpan(date, date.AddDays(1), DigitalstromSceneEventData.MaxEventsPerDay));
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }

        /// <summary>
        /// Saves the event stream to the database by updating an existing row or creating a new one if
        /// not existing yet.
        /// </summary>
        private void SaveEventStreamToDb()
        {
            var date = _eventStream.Span.Begin.Date;

            _dbContext.Semaphore.WaitOne();
            try
            {
                var dbSceneEvents = _dbContext.DsSceneEventDataSet.Where(x => x.Day == date).FirstOrDefault();
                if (dbSceneEvents == null)
                    _dbContext.DsSceneEventDataSet.Add(dbSceneEvents = new DigitalstromSceneEventData() { Day = date });

                dbSceneEvents.EventStream = _eventStream;

                _dbContext.SaveChanges();
                _logger.LogInformation($"{DateTime.Now} Persisted event clump");
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }
    }
}