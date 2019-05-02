using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class ZoneStructure
    {
        public Zone Id { get; set; }
        public string Name { get; set; }
        public bool IsPresent { get; set; }
        public List<Device> Devices { get; set; }
        public List<GroupStructure> Groups { get; set; }
    }
}