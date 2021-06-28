using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Network;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class DssEventSubscriber : LongPollingClientBase<DssEvent>
    {
        private class DigitalstromLongPollingConnectionProvider : IDigitalstromConnectionProvider
        {
            private readonly IDigitalstromConnectionProvider inner;

            public DigitalstromLongPollingConnectionProvider(IDigitalstromConnectionProvider inner) => this.inner = inner;

            UriPriorityList IDigitalstromConnectionProvider.Uris => inner.Uris;

            IDigitalstromAuth IDigitalstromConnectionProvider.AuthData => inner.AuthData;

            HttpClient IDigitalstromConnectionProvider.HttpClient => inner.LongPollingHttpClient;

            HttpClient IDigitalstromConnectionProvider.LongPollingHttpClient => inner.LongPollingHttpClient;
        }

        private const int defaultSubscriptionId = 10;

        private readonly SemaphoreSlim initializedSempahore = new(0, 1);
        private volatile bool subscribed;
        private readonly int subscriptionId;

        private DigitalstromDssClient? apiClient;
        private readonly bool ownsClient = false;

        private readonly IEnumerable<IEventName> eventsToSubscribe;

        public DssEventSubscriber(IDigitalstromConnectionProvider connectionProvider, IEnumerable<IEventName> eventsToSubscribe, int subscriptionId = defaultSubscriptionId)
            : this(new DigitalstromDssClient(new DigitalstromLongPollingConnectionProvider(connectionProvider)), eventsToSubscribe, subscriptionId)
        {
            ownsClient = true;
        }

        public DssEventSubscriber(DigitalstromDssClient dssClient, IEnumerable<IEventName> eventsToSubscribe, int subscriptionId = defaultSubscriptionId)
        {
            apiClient = dssClient;
            this.subscriptionId = subscriptionId;
            this.eventsToSubscribe = eventsToSubscribe;

            subscribed = false;
            initializedSempahore.Release();
        }

        protected override async Task<IEnumerable<DssEvent>> ProcessEventPolling()
        {
            if (apiClient is null)
                return new List<DssEvent>();

            await Subscribe();

            var events = await apiClient.PollForEvents(subscriptionId, 60000);
            if (events.Events == null)
                return new List<DssEvent>();

            return events.Events;
        }

        private async Task Subscribe()
        {
            if (subscribed || apiClient is null)
                return;

            await initializedSempahore.WaitAsync();
            try
            {
                foreach (var eventName in eventsToSubscribe)
                    await apiClient.Subscribe(eventName, subscriptionId);
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