using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class DssEventProperties
    {
        public string CallOrigin { get; set; }
        public Scene SceneID { get; set; }
        public Group GroupID { get; set; }
        public Zone ZoneID { get; set; }
        public bool? Forced { get; set; }
        public string OriginToken { get; set; }
        public string OriginDSUID { get; set; }
    }
}