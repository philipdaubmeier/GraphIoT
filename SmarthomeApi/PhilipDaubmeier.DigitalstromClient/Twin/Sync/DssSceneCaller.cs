using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class DssSceneCaller
    {
        private readonly DssEventSubscriber apiClient;

        private readonly ApartmentState model;

        public DssSceneCaller(DssEventSubscriber client, ApartmentState model)
        {
            apiClient = client;
            this.model = model;

            model.CollectionChanged += ApartmentCollectionChanged;
        }

        private void ApartmentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems.Count < 1)
                return;

            var roomState = (e.NewItems[0] as KeyValuePair<Zone, RoomState>?)?.Value;
            if (roomState == null)
                return;

            roomState.CollectionChanged += RoomStateCollectionChanged;
        }

        private void RoomStateCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void CallScene(Zone zone, Group group, Scene scene)
        {
            apiClient.CallScene(zone, group, scene);
        }
    }
}