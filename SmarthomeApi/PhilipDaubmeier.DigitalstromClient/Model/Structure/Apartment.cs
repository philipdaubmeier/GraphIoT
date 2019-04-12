using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class Apartment
    {
        public List<Cluster> clusters { get; set; }
        public List<ZoneStructure> zones { get; set; }
    }
}
