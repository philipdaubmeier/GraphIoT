using PhilipDaubmeier.DigitalstromClient.Network;
using System;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class DigitalstromTwin : ApartmentState, IDisposable
    {
        private readonly DssEventSubscriber _subscriber;
        private readonly DssSceneCaller _sceneCaller;

        public DigitalstromTwin(IDigitalstromConnectionProvider connectionProvider)
        {
            _subscriber = connectionProvider is null ? null : new DssEventSubscriber(connectionProvider, this);
            _sceneCaller = _subscriber is null ? null : new DssSceneCaller(_subscriber, this);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}