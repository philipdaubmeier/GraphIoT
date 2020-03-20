using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureList
    {
        public List<Feature>? Features { get; set; }

        public PropertyList? GetFeature(FeatureName.Name name, FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            var nameStr = (string)new FeatureName(name, circuit);
            return Features.Where(x => x.Name?.Equals(nameStr, System.StringComparison.InvariantCultureIgnoreCase) ?? false).FirstOrDefault()?.Properties;
        }
    }
}