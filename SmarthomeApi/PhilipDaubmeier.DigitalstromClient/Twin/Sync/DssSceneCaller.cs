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

            model.CollectionChanged += ModelCollectionChanged;
        }

        private void ModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems.Count < 1)
                return;

            var addedItem = e.NewItems[0] as KeyValuePair<Zone, RoomState>?;
            if (addedItem == null)
                return;

            // TODO: subscribe to new item
        }

        public void CallScene(Zone zone, Group group, Scene scene)
        {
            apiClient.CallScene(zone, group, scene);
        }
    }
}