using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class Apartment
    {
        public List<Cluster> Clusters { get; set; } = new List<Cluster>();
        public List<ZoneStructure> Zones { get; set; } = new List<ZoneStructure>();
    }
}