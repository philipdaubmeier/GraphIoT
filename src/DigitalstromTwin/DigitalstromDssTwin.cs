using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class DigitalstromDssTwin : ApartmentState, IDisposable
    {
        private readonly DigitalstromDssClient? _dssClient;
        private readonly DssEventSubscriber? _subscriber;
        private readonly TwinChangeAggregator? _changeAggregator;

        public event SceneChangedEventHandler SceneChanged
        {
            add { if (_changeAggregator != null) _changeAggregator.SceneChanged += value; }
            remove { if (_changeAggregator != null) _changeAggregator.SceneChanged -= value; }
        }

        public DigitalstromDssTwin(IDigitalstromConnectionProvider connectionProvider)
        {
            _dssClient = connectionProvider is null ? null : new DigitalstromDssClient(connectionProvider);
            _subscriber = _dssClient is null ? null : new DssEventSubscriber(_dssClient, SubscribedEventNames);
            _changeAggregator = _subscriber is null ? null : new TwinChangeAggregator(this);

            if (_subscriber is null || _changeAggregator is null)
                return;

            _subscriber.ApiEventRaised += HandleDssApiEvent;
            _changeAggregator.SceneChangedInternal += (s, e) => CallScene(e.Zone, e.Group, e.Scene);

            // init by loading all current values for all zones
            LoadApartmentScenes();
            LoadApartmentSensors();
        }

        private async void LoadApartmentScenes()
        {
            if (_dssClient is null)
                return;

            var apartment = await _dssClient.GetZonesAndLastCalledScenes();
            if (apartment?.Zones is null)
                return;

            foreach (var zone in apartment.Zones.Where(x => x?.Groups != null))
                foreach (var groupstructure in zone.Groups.Where(x => x != null))
                    this[zone.ZoneID, groupstructure.Group].ValueInternal = groupstructure.LastCalledScene;
        }

        private async void LoadApartmentSensors()
        {
            if (_dssClient is null)
                return;

            var apartment = await _dssClient.GetZonesAndSensorValues();
            if (apartment?.Zones is null)
                return;

            foreach (var zone in apartment.Zones.Where(x => x?.Sensor != null))
                foreach (var sensor in zone.Sensor.Where(x => x != null))
                    this[zone.ZoneID, sensor.Type].ValueInternal = sensor;
        }

        private async void CallScene(Zone zone, Group group, Scene scene)
        {
            if (_dssClient is null)
                return;

            await _dssClient.CallScene(zone, group, scene);
        }

        private IEnumerable<IEventName> SubscribedEventNames
        {
            get
            {
                yield return (SystemEventName)SystemEvent.CallScene;
                yield return (SystemEventName)SystemEvent.CallSceneBus;
            }
        }

        private void HandleDssApiEvent(object? sender, ApiEventRaisedEventArgs<DssEvent> args)
        {
            if (args.ApiEvent is null)
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
            if (props is null)
                return;

            this[props.ZoneID, props.GroupID].ValueInternal = props.SceneID;
        }

        public void Dispose()
        {
            _changeAggregator?.Dispose();
            _subscriber?.Dispose();
            _dssClient?.Dispose();
        }
    }
}