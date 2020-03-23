using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureList : List<Feature>
    {
        public DateTime GetTimestamp()
        {
            return this.FirstOrDefault()?.Timestamp ?? DateTime.UtcNow;
        }

        public Feature? GetRawFeatureByName(string featureName)
        {
            return this.Where(x => x.Name?.Equals(featureName, StringComparison.InvariantCultureIgnoreCase) ?? false).FirstOrDefault();
        }

        protected PropertyList? GetProperties(FeatureName.Name name, FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetRawFeatureByName(new FeatureName(name, circuit))?.Properties;
        }
    }
}