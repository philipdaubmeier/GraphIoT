using PhilipDaubmeier.DigitalstromClient.Network;
using PhilipDaubmeier.DigitalstromClient.Twin;
using System;

namespace PhilipDaubmeier.DigitalstromClient
{
    public class DigitalstromDssTwin : ApartmentState, IDisposable
    {
        private readonly DssEventSubscriber _subscriber;
        private readonly DssSceneCaller _sceneCaller;

        public DigitalstromDssTwin(IDigitalstromConnectionProvider connectionProvider)
        {
            _subscriber = connectionProvider is null ? null : new DssEventSubscriber(connectionProvider, this);
            _sceneCaller = _subscriber is null ? null : new DssSceneCaller(_subscriber, this);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
            _sceneCaller?.Dispose();
        }
    }
}