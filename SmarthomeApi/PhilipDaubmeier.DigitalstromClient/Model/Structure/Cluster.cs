using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class Cluster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Group Color { get; set; }
        public int ApplicationType { get; set; }
        public bool IsPresent { get; set; }
        public bool IsValid { get; set; }
        public string CardinalDirection { get; set; }
        public int ProtectionClass { get; set; }
        public bool IsAutomatic { get; set; }
        public bool ConfigurationLock { get; set; }
        public List<DSUID> Devices { get; set; }
    }
}