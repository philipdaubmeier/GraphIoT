using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class DssEventSubscriber : WebApiWorkerThreadBase<DssEvent>
    {
        private const int defaultSubscriptionId = 10;

        public event EventHandler ModelChanged;

        private volatile bool subscribed;
        private readonly int subscriptionId;
        private DigitalstromWebserviceClient apiClient;

        private readonly IEnumerable<IEventName> eventsToSubscribe;

        public ApartmentState Scenes { get; set; }

        public DssEventSubscriber(IDigitalstromConnectionProvider connectionProvider, ApartmentState model = null, IEnumerable<IEventName> eventsToSubscribe = null, int subscriptionId = defaultSubscriptionId)
        {
            apiClient = new DigitalstromWebserviceClient(connectionProvider);
            Scenes = model ?? new ApartmentState();
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
            if (apartment?.Zones == null)
                return;

            foreach (var zone in apartment.Zones.Where(x => x?.Groups != null))
                foreach (var groupstructure in zone.Groups.Where(x => x != null))
                    Scenes[zone.ZoneID, groupstructure.Group].Value = groupstructure.LastCalledScene;
        }

        private async Task LoadApartmentSensors()
        {
            var apartment = await apiClient?.GetZonesAndSensorValues();
            if (apartment?.Zones == null)
                return;

            foreach (var zone in apartment.Zones.Where(x => x?.Sensor != null))
                foreach (var sensor in zone.Sensor.Where(x => x != null))
                    Scenes[zone.ZoneID, sensor.Type].Value = sensor;
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

            Scenes[props.ZoneID, props.GroupID].ValueInternal = props.SceneID;
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