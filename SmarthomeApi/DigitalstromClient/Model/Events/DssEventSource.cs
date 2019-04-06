namespace DigitalstromClient.Model.Events
{
    public class DssEventSource
    {
        public string set { get; set; }
        public int groupID { get; set; }
        public int zoneID { get; set; }
        public bool isApartment { get; set; }
        public bool isGroup { get; set; }
        public bool isDevice { get; set; }
    }
}
