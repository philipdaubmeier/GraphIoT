using DigitalstromClient.Model.Core;

namespace DigitalstromClient.Model.Events
{
    public class DssEventProperties
    {
        public string callOrigin { get; set; }
        public string sceneID { get; set; }
        public string groupID { get; set; }
        public string zoneID { get; set; }
        public string originToken { get; set; }
        public string originDSUID { get; set; }

        public Scene scene { get { return sceneID; } }
        public Group group { get { return groupID; } }
        public Zone zone { get { return zoneID; } }
    }
}