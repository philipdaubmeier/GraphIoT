using System.Collections.Generic;

namespace DigitalstromClient.Model.Structure
{
    public class Apartment
    {
        public List<Cluster> clusters { get; set; }
        public List<ZoneStructure> zones { get; set; }
    }
}
