using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    internal class DssSceneCaller : IDisposable
    {
        private readonly ApartmentState model;

        public event SceneChangedEventHandler SceneChanged;

        internal event SceneChangedEventHandler SceneChangedInternal;

        public DssSceneCaller(ApartmentState model)
        {
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
            {
                item.Value.PropertyChanged += StatePropertyChanged;
                item.Value.PropertyChangedInternal += StatePropertyChangedInternal;
            }

            foreach (var item in room.Sensors)
            {
                item.Value.PropertyChanged += StatePropertyChanged;
                item.Value.PropertyChangedInternal += StatePropertyChangedInternal;
            }
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
            {
                sensorState.PropertyChanged += StatePropertyChanged;
                sensorState.PropertyChangedInternal += StatePropertyChangedInternal;
            }

            var sceneState = (e.NewItems[0] as KeyValuePair<Group, SceneState>?)?.Value;
            if (sceneState != null)
            {
                sceneState.PropertyChanged += StatePropertyChanged;
                sceneState.PropertyChangedInternal += StatePropertyChangedInternal;
            }
        }

        private void StatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // ignore sensor state changes, as sensors can not be changed/written
            var sceneState = sender as SceneState;
            if (sceneState is null)
                return;

            if (e.PropertyName != nameof(SceneState.Value))
                return;

            if (!TryFindZoneAndGroup(sceneState, out Zone zone, out Group group))
                return;

            SceneChanged?.Invoke(model, new SceneChangedEventArgs()
            {
                Zone = zone,
                Group = group,
                Scene = sceneState.Value
            });
        }

        private void StatePropertyChangedInternal(object sender, PropertyChangedEventArgs e)
        {
            var sceneState = sender as SceneState;
            if (sceneState is null)
                return;

            if (!TryFindZoneAndGroup(sceneState, out Zone zone, out Group group))
                return;

            SceneChangedInternal?.Invoke(model, new SceneChangedEventArgs()
            {
                Zone = zone,
                Group = group,
                Scene = sceneState.Value
            });
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