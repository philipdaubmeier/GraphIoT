using PhilipDaubmeier.DigitalstromClient.Twin;
using System;

namespace PhilipDaubmeier.DigitalstromClient
{
    public class DigitalstromDssTwin : ApartmentState, IDisposable
    {
        private readonly DssEventSubscriber _subscriber;
        private readonly TwinChangeAggregator _changeAggregator;

        public event SceneChangedEventHandler SceneChanged
        {
            add { _changeAggregator.SceneChanged += value; }
            remove { _changeAggregator.SceneChanged -= value; }
        }

        public DigitalstromDssTwin(IDigitalstromConnectionProvider connectionProvider)
        {
            _subscriber = connectionProvider is null ? null : new DssEventSubscriber(connectionProvider, this);
            _changeAggregator = _subscriber is null ? null : new TwinChangeAggregator(this);

            _changeAggregator.SceneChangedInternal += (s, e) => _subscriber.CallScene(e.Zone, e.Group, e.Scene);
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
            _changeAggregator?.Dispose();
        }
    }
}