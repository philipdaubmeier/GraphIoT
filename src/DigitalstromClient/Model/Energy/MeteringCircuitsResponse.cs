using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class MeteringCircuitsResponse : IWiremessagePayload
    {
        public List<MeteringCapabilities> DSMeters { get; set; } = new List<MeteringCapabilities>();

        /// <summary>
        /// Returns the DSUID and name of all circuits that have metering capability
        /// </summary>
        [JsonIgnore]
        public Dictionary<Dsuid, string> FilteredMeterNames =>
                DSMeters.Where(x => x.Capabilities.FirstOrDefault()?.Metering ?? false)
                .ToDictionary(x => x.DSUID, x => x.Name);
    }

    public class MeteringCapabilities
    {
        public Dsuid DSUID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<MeteringCapability> Capabilities { get; set; } = new List<MeteringCapability>();
    }

    public class MeteringCapability
    {
        public bool Metering { get; set; }
    }
}