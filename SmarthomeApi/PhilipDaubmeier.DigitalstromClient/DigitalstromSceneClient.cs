using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Model.RoomState;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient
{
    public class DigitalstromSceneClient : WebApiWorkerThreadBase<DssEvent>
    {
        private const int defaultSubscriptionId = 10;

        public event EventHandler ModelChanged;

        private volatile bool subscribed;
        private readonly int subscriptionId;
        private DigitalstromWebserviceClient apiClient;

        private readonly IEnumerable<IEventName> eventsToSubscribe;

        public ApartmentState Scenes { get; set; }

        public DigitalstromSceneClient(IDigitalstromConnectionProvider connectionProvider, IEnumerable<IEventName> eventsToSubscribe = null, int subscriptionId = defaultSubscriptionId)
        {
            apiClient = new DigitalstromWebserviceClient(connectionProvider);
            Scenes = new ApartmentState();
            ApiEventRaised += HandleDssApiEvent;
            this.subscriptionId = subscriptionId;
            subscribed = false;

            this.eventsToSubscribe = new List<IEventName>()
            {
                (SystemEventName)SystemEvent.CallScene,
                (SystemEventName)SystemEvent.CallSceneBus
            };
            if (eventsToSubscribe != null)
                ((List<IEventName>)this.eventsToSubscribe).AddRange(eventsToSubscribe);

            LoadApartment();
        }

        private async void LoadApartment()
        {
            await LoadApartmentScenes();
            await LoadApartmentSensors();

            OnModelChanged();
        }

        private async Task LoadApartmentScenes()
        {
            var apartment = await apiClient?.GetZonesAndLastCalledScenes();
            if (apartment == null || apartment.Zones == null)
                return;

            foreach (var zone in apartment.Zones)
            {
                if (zone == null || zone.Groups == null || zone.Groups.Count <= 0)
                    continue;

                RoomState roomState = new RoomState();
                foreach (var groupstructure in zone.Groups)
                {
                    if (groupstructure == null)
                        continue;

                    roomState[groupstructure.Group].Value = groupstructure.LastCalledScene;
                }
                Scenes[zone.ZoneID] = roomState;
            }
        }

        private async Task LoadApartmentSensors()
        {
            var apartment = await apiClient?.GetZonesAndSensorValues();
            if (apartment == null || apartment.Zones == null)
                return;

            foreach (var zone in apartment.Zones)
            {
                if (zone == null || zone.Sensor == null || zone.Sensor.Count <= 0)
                    continue;

                RoomState roomState = Scenes[zone.ZoneID];
                if (roomState == null)
                    roomState = new RoomState();
                foreach (var sensor in zone.Sensor)
                {
                    if (sensor == null)
                        continue;

                    roomState[sensor.Type].Value = sensor;
                }
                Scenes[zone.ZoneID] = roomState;
            }
        }

        public void CallScene(Zone zone, Group group, Scene scene)
        {
            QueueAction(async () => await apiClient?.CallScene(zone, group, scene));
        }

        protected override async Task<IEnumerable<DssEvent>> ProcessEventPolling()
        {
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

            Scenes[props.ZoneID, props.GroupID].Value = props.SceneID;
            OnModelChanged();
        }

        private void OnModelChanged()
        {
            if (ModelChanged == null)
                return;

            ModelChanged(this, null);
        }

        public new void Dispose()
        {
            apiClient?.Dispose();
            apiClient = null;

            base.Dispose();
        }
    }
}