using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
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
            throw new NotImplementedException();
        }

        public void CallScene(Zone zone, Group group, Scene scene)
        {
            apiClient.CallScene(zone, group, scene);
        }
    }
}