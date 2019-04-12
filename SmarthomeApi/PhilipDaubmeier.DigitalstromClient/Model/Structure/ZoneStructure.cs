using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class ZoneStructure
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isPresent { get; set; }
        public List<Device> devices { get; set; }
        public List<GroupStructure> groups { get; set; }
    }
}
