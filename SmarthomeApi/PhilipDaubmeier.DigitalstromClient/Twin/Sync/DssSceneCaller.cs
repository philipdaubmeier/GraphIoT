using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class DssSceneCaller : IDisposable
    {
        private readonly DssEventSubscriber apiClient;

        private readonly ApartmentState model;

        public DssSceneCaller(DssEventSubscriber client, ApartmentState model)
        {
            apiClient = client;
            this.model = model;

            SubscribeApartment(model);
        }

        public void Dispose() { }

        private void SubscribeApartment(ApartmentState model)
        {
            model.CollectionChanged += ApartmentCollectionChanged;

            foreach (var room in model)
            {
                room.Value.CollectionChanged += RoomStateCollectionChanged;
                SubscribeChangesRoom(room.Value);
            }
        }

        private void SubscribeChangesRoom(RoomState room)
        {
            foreach (var item in room.Groups)
                item.Value.PropertyChangedInternal += StatePropertyChanged;

            foreach (var item in room.Sensors)
                item.Value.PropertyChangedInternal += StatePropertyChanged;
        }

        private void ApartmentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems.Count < 1)
                return;

            var roomState = (e.NewItems[0] as KeyValuePair<Zone, RoomState>?)?.Value;
            if (roomState == null)
                return;

            roomState.CollectionChanged += RoomStateCollectionChanged;
            SubscribeChangesRoom(roomState);
        }

        private void RoomStateCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems.Count < 1)
                return;

            var sensorState = (e.NewItems[0] as KeyValuePair<Sensor, SensorState>?)?.Value;
            if (sensorState != null)
                sensorState.PropertyChangedInternal += StatePropertyChanged;

            var sceneState = (e.NewItems[0] as KeyValuePair<Group, SceneState>?)?.Value;
            if (sceneState != null)
                sceneState.PropertyChangedInternal += StatePropertyChanged;
        }

        private void StatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // ignore sensor state changes, as sensors can not be changed/written
            var sceneState = sender as SceneState;
            if (sceneState is null)
                return;

            if (!TryFindZoneAndGroup(sceneState, out Zone zone, out Group group))
                return;

            apiClient.CallScene(zone, group, sceneState.Value);
        }

        private bool TryFindZoneAndGroup(SceneState sceneState, out Zone zone, out Group group)
        {
            foreach (var room in model)
            {
                foreach (var groupState in room.Value.Groups)
                {
                    if (groupState.Value != sceneState)
                        continue;

                    zone = room.Key;
                    group = groupState.Key;
                    return true;
                }
            }
            zone = default;
            group = default;
            return false;
        }
    }
}