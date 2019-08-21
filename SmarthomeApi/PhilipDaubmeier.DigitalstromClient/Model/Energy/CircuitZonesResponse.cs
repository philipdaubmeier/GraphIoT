using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class CircuitZonesResponse : IWiremessagePayload
    {
        public List<MeterZones> DSMeters { get; set; }
    }

    public class MeterZones
    {
        public Dsuid DSUID { get; set; }
        public List<MeterZone> Zones { get; set; }
    }

    public class MeterZone
    {
        public Zone ZoneID { get; set; }
    }
}