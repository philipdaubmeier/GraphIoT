using DigitalstromClient.Model;
using DigitalstromClient.Model.Events;
using DigitalstromClient.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmarthomeApi.Services
{
    public class DigitalstromEventsHostedService : IHostedService, IDisposable
    {
        private static List<string> receivedEvents = new List<string>();

        private readonly ILogger _logger;
        private readonly PersistenceContext _dbContext;
        private readonly IDigitalstromConnectionProvider _connProvider;
        private DigitalstromSceneClient _dsSceneClient = null;
        
        public DigitalstromEventsHostedService(ILogger<ViessmannTimedHostedService> logger, PersistenceContext databaseContext, IDigitalstromConnectionProvider connectionProvider)
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
            
            _dsSceneClient = new DigitalstromSceneClient(_connProvider);
            _dsSceneClient.ApiEventRaised += (s, e)=>DigitalstromEventReceived(e.ApiEvent);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Event Subscriber Service is stopping.");

            _dsSceneClient.Dispose();
            _dsSceneClient = null;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // DigitalstromSceneClient.Dispose stops all its threads
            _dsSceneClient?.Dispose();
        }
        
        private void DigitalstromEventReceived(DssEvent dsEvent)
        {
            receivedEvents.Add($"{DateTime.Now} name '{dsEvent.name}' zone '{dsEvent.properties.zone}' group '{dsEvent.properties.group}' scene '{dsEvent.properties.scene}'");
        }
    }
}