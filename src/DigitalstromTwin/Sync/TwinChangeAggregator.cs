using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PhilipDaubmeier.DigitalstromTwin
{
    internal class TwinChangeAggregator : IDisposable
    {
        private readonly ApartmentState model;

        public event SceneChangedEventHandler? SceneChanged;

        internal event SceneChangedEventHandler? SceneChangedInternal;

        /// <summary>
        /// Creates a new TwinChangeAggregator instance for the given twin model. It subscribes
        /// to all CollectionChanged and PropertyChanged events deep down the whole model tree
        /// and keeps the subscriptions up-to-date (i.e. if a new room is added, this addition of
        /// the room will trigger a CollectionChanged which will in turn be used to subscribe to
        /// all child elements and their PropertyChanged events). Any change to a sceene in the
        /// model is therefore aggregated into a single SceneChanged event.
        /// </summary>
        public TwinChangeAggregator(ApartmentState model)
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
            if (!(sender is SceneState sceneState))
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
            if (!(sender is SceneState sceneState))
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
            zone = 0;
            group = 0;
            return false;
        }
    }
}