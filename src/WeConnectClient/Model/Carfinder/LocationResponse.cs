using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Carfinder
{
    internal class LocationResponse : Wiremessage<Location>
    {
        [JsonPropertyName("position")]
        public override Location Body { get; set; } = new Location();
    }
}