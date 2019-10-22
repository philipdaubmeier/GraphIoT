using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class DssEventProperties
    {
        public string CallOrigin { get; set; } = string.Empty;
        public Scene SceneID { get; set; } = SceneCommand.Unknown;
        public Group GroupID { get; set; } = Color.White;
        public Zone ZoneID { get; set; } = 0;
        public bool? Forced { get; set; }
        public string? OriginToken { get; set; }
        public string? OriginDSUID { get; set; }
    }
}