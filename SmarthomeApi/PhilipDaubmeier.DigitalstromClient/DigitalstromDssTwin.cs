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
            _sceneCaller = _subscriber is null ? null : new DssSceneCaller(this);

            _sceneCaller.SceneChangedInternal += (s, e) => _subscriber.CallScene(e.Zone, e.Group, e.Scene);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
            _sceneCaller?.Dispose();
        }
    }
}