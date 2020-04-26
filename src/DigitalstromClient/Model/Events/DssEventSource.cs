using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class DssEventSource
    {
        public string Set { get; set; } = string.Empty;
        public Group GroupID { get; set; } = Color.White;
        public Zone ZoneID { get; set; } = 0;
        public bool IsApartment { get; set; }
        public bool IsGroup { get; set; }
        public bool IsDevice { get; set; }
    }
}