using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class CircuitZonesResponse : IWiremessagePayload<CircuitZonesResponse>
    {
        public List<MeterZones> DSMeters { get; set; }
    }

    public class MeterZones
    {
        public string DSUID { get; set; }
        public List<MeterZone> zones { get; set; }
    }

    public class MeterZone
    {
        public int ZoneID { get; set; }
    }
}