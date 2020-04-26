using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Network;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class DssEventProperties
    {
        public string CallOrigin { get; set; } = string.Empty;
        public Scene SceneID { get; set; } = SceneCommand.Unknown;
        public Group GroupID { get; set; } = Color.White;
        public Zone ZoneID { get; set; } = 0;

        [JsonConverter(typeof(StringToBoolConverter))]
        public bool Forced { get; set; } = false;
        
        public string? OriginToken { get; set; }
        public string? OriginDSUID { get; set; }
    }
}