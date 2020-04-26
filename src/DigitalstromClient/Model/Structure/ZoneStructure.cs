using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class ZoneStructure
    {
        public Zone Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public bool IsPresent { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
        public List<GroupStructure> Groups { get; set; } = new List<GroupStructure>();
    }
}