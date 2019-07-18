using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class DssEventSubscriber : LongPollingClientBase<DssEvent>
    {
        private const int defaultSubscriptionId = 10;

        private readonly SemaphoreSlim initializedSempahore = new SemaphoreSlim(0, 1);
        private volatile bool subscribed;
        private readonly int subscriptionId;

        private DigitalstromDssClient apiClient;
        private readonly bool ownsClient = false;

        private readonly IEnumerable<IEventName> eventsToSubscribe;

        public DssEventSubscriber(IDigitalstromConnectionProvider connectionProvider, IEnumerable<IEventName> eventsToSubscribe = null, int subscriptionId = defaultSubscriptionId)
            : this(new DigitalstromDssClient(connectionProvider), eventsToSubscribe, subscriptionId)
        {
            ownsClient = true;
        }

        public DssEventSubscriber(DigitalstromDssClient dssClient, IEnumerable<IEventName> eventsToSubscribe = null, int subscriptionId = defaultSubscriptionId)
        {
            apiClient = dssClient;
            this.subscriptionId = subscriptionId;

            this.eventsToSubscribe = new List<IEventName>()
            {
                (SystemEventName)SystemEvent.CallScene,
                (SystemEventName)SystemEvent.CallSceneBus
            };
            if (eventsToSubscribe != null)
                ((List<IEventName>)this.eventsToSubscribe).AddRange(eventsToSubscribe.Except(this.eventsToSubscribe));

            subscribed = false;
            initializedSempahore.Release();
        }

        protected override async Task<IEnumerable<DssEvent>> ProcessEventPolling()
        {
            await Subscribe();

            var events = await apiClient?.PollForEvents(subscriptionId, 60000);
            if (events.Events == null)
                return new List<DssEvent>();

            return events.Events;
        }

        private async Task Subscribe()
        {
            if (subscribed)
                return;
            
            await initializedSempahore.WaitAsync();
            try
            {
                foreach (var eventName in eventsToSubscribe)
                    await apiClient?.Subscribe(eventName, subscriptionId);
                subscribed = true;
            }
            finally
            {
                initializedSempahore.Release();
            }
        }

        #region IDisposable Support
        private bool isDisposed = false;

        public new void Dispose()
        {
            if (isDisposed)
                return;

            if (ownsClient)
            {
                apiClient?.Dispose();
                apiClient = null;
            }

            base.Dispose();

            isDisposed = true;
        }
        #endregion
    }
}