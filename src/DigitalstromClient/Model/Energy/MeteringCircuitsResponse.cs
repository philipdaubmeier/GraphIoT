using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class MeteringCircuitsResponse : IWiremessagePayload
    {
        public List<MeteringCapabilities> DSMeters { get; set; }

        /// <summary>
        /// Returns the DSUID and name of all circuits that have metering capability
        /// </summary>
        public Dictionary<Dsuid, string> FilteredMeterNames =>
                DSMeters.Where(x => x.Capabilities.FirstOrDefault()?.Metering ?? false)
                .ToDictionary(x => x.DSUID, x => x.Name);
    }

    public class MeteringCapabilities
    {
        public Dsuid DSUID { get; set; }
        public string Name { get; set; }
        public List<MeteringCapability> Capabilities { get; set; }
    }

    public class MeteringCapability
    {
        public bool Metering { get; set; }
    }
}