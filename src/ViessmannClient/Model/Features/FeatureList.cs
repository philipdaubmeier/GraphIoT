using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureList : List<Feature>
    {
        public PropertyList? GetFeature(FeatureName.Name name, FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetFirstFeature(name, circuit)?.Properties;
        }

        public DateTime? GetTimestamp(FeatureName.Name name, FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetFirstFeature(name, circuit)?.Timestamp;
        }

        private Feature GetFirstFeature(FeatureName.Name name, FeatureName.Circuit? circuit)
        {
            var nameStr = (string)new FeatureName(name, circuit);
            return this.Where(x => x.Name?.Equals(nameStr, StringComparison.InvariantCultureIgnoreCase) ?? false).FirstOrDefault();
        }
    }
}