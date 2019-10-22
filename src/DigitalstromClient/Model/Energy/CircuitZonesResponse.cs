using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class CircuitZonesResponse : IWiremessagePayload
    {
        public List<MeterZones> DSMeters { get; set; } = new List<MeterZones>();
    }

    public class MeterZones
    {
        public Dsuid DSUID { get; set; } = string.Empty;
        public List<MeterZone> Zones { get; set; } = new List<MeterZone>();
    }

    public class MeterZone
    {
        public Zone ZoneID { get; set; } = 0;
    }
}