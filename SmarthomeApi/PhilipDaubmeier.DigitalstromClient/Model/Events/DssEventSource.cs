using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class DssEventSource
    {
        public string Set { get; set; }
        public Group GroupID { get; set; }
        public Zone ZoneID { get; set; }
        public bool IsApartment { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDevice { get; set; }
    }
}