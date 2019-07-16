using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class DssEventSubscriber : WebApiWorkerThreadBase<DssEvent>
    {
        private const int defaultSubscriptionId = 10;

        private readonly Semaphore initializedSempahore = new Semaphore(0, 1);
        private volatile bool subscribed;
        private readonly int subscriptionId;
        private DigitalstromDssClient apiClient;

        private readonly IEnumerable<IEventName> eventsToSubscribe;

        public DssEventSubscriber(IDigitalstromConnectionProvider connectionProvider, IEnumerable<IEventName> eventsToSubscribe = null, int subscriptionId = defaultSubscriptionId)
        {
            apiClient = new DigitalstromDssClient(connectionProvider);
            this.subscriptionId = subscriptionId;
            subscribed = false;

            this.eventsToSubscribe = new List<IEventName>()
            {
                (SystemEventName)SystemEvent.CallScene,
                (SystemEventName)SystemEvent.CallSceneBus
            };
            if (eventsToSubscribe != null)
                ((List<IEventName>)this.eventsToSubscribe).AddRange(eventsToSubscribe.Except(this.eventsToSubscribe));

            initializedSempahore.Release();
        }

        public void CallDss(Func<DigitalstromDssClient, Task> callAction)
        {
            QueueAction(async () => await callAction(apiClient));
        }

        protected override async Task<IEnumerable<DssEvent>> ProcessEventPolling()
        {
            initializedSempahore.WaitOne();

            if (!subscribed)
            {
                foreach (var eventName in eventsToSubscribe)
                    await apiClient?.Subscribe(eventName, subscriptionId);
                subscribed = true;
            }
            var events = await apiClient?.PollForEvents(subscriptionId, 60000);
            if (events.Events == null)
                return new List<DssEvent>();

            return events.Events;
        }

        public new void Dispose()
        {
            apiClient?.Dispose();
            apiClient = null;

            base.Dispose();
        }
    }
}