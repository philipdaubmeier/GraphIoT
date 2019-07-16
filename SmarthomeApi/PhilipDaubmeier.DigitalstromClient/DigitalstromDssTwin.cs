using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Network;
using PhilipDaubmeier.DigitalstromClient.Twin;
using System;
using System.Linq;

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
            _subscriber = connectionProvider is null ? null : new DssEventSubscriber(connectionProvider);
            _changeAggregator = _subscriber is null ? null : new TwinChangeAggregator(this);

            _subscriber.ApiEventRaised += HandleDssApiEvent;
            _changeAggregator.SceneChangedInternal += (s, e) => CallScene(e.Zone, e.Group, e.Scene);

            // init by loading all current values for all zones
            LoadApartmentScenes();
            LoadApartmentSensors();
        }

        private void LoadApartmentScenes()
        {
            _subscriber.CallDss(async dssClient =>
            {
                var apartment = await dssClient?.GetZonesAndLastCalledScenes();
                if (apartment?.Zones == null)
                    return;

                foreach (var zone in apartment.Zones.Where(x => x?.Groups != null))
                    foreach (var groupstructure in zone.Groups.Where(x => x != null))
                        this[zone.ZoneID, groupstructure.Group].ValueInternal = groupstructure.LastCalledScene;
            });
        }

        private void LoadApartmentSensors()
        {
            _subscriber.CallDss(async dssClient =>
            {
                var apartment = await dssClient?.GetZonesAndSensorValues();
                if (apartment?.Zones == null)
                    return;

                foreach (var zone in apartment.Zones.Where(x => x?.Sensor != null))
                    foreach (var sensor in zone.Sensor.Where(x => x != null))
                        this[zone.ZoneID, sensor.Type].ValueInternal = sensor;
            });
        }

        private void CallScene(Zone zone, Group group, Scene scene)
        {
            _subscriber.CallDss(async dssClient => await dssClient?.CallScene(zone, group, scene));
        }

        private void HandleDssApiEvent(object sender, ApiEventRaisedEventArgs<DssEvent> args)
        {
            if (args.ApiEvent == null)
                return;

            switch (args.ApiEvent.SystemEvent.Type)
            {
                case SystemEvent.CallSceneBus: goto case SystemEvent.CallScene;
                case SystemEvent.CallScene:
                    HandleDssCallSceneEvent(args.ApiEvent); break;
            }
        }

        private void HandleDssCallSceneEvent(DssEvent dssEvent)
        {
            var props = dssEvent.Properties;
            if (props == null)
                return;

            this[props.ZoneID, props.GroupID].ValueInternal = props.SceneID;
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
            _changeAggregator?.Dispose();
        }
    }
}