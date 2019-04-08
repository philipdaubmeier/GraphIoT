using DigitalstromClient.Model.Core;
using System.Collections.Generic;
using System.Linq;

namespace DigitalstromClient.Model.Energy
{
    public class MeteringCircuitsResponse : IWiremessagePayload<MeteringCircuitsResponse>
    {
        public List<MeteringCapabilities> DSMeters { get; set; }

        /// <summary>
        /// Returns the DSUID and name of all circuits that have metering capability
        /// </summary>
        public Dictionary<DSUID, string> FilteredMeterNames =>
                DSMeters.Where(x => x.Capabilities.FirstOrDefault()?.Metering ?? false)
                .Select(x => new KeyValuePair<string, string>(x.DSUID, x.Name))
                .ToDictionary(x => new DSUID(x.Key), x => x.Value);
    }

    public class MeteringCapabilities
    {
        public string DSUID { get; set; }
        public string Name { get; set; }
        public List<MeteringCapability> Capabilities { get; set; }
    }

    public class MeteringCapability
    {
        public bool Metering { get; set; }
    }
}