using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Geofence
{
    internal class GeofenceResponse : Wiremessage<GeofenceCollection>
    {
        [JsonPropertyName("geoFenceResponse")]
        public override GeofenceCollection Body { get; set; } = new GeofenceCollection();
    }
}