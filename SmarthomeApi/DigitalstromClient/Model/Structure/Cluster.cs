using System.Collections.Generic;

namespace DigitalstromClient.Model.Structure
{
    public class Cluster
    {
        public int id { get; set; }
        public string name { get; set; }
        public int color { get; set; }
        public int applicationType { get; set; }
        public bool isPresent { get; set; }
        public bool isValid { get; set; }
        public string CardinalDirection { get; set; }
        public int ProtectionClass { get; set; }
        public bool isAutomatic { get; set; }
        public bool configurationLock { get; set; }
        public List<string> devices { get; set; }
    }
}
