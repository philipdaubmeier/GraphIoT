using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Carfinder
{
    public class Location
    {
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [JsonPropertyName("lng")]
        public double Longitude { get; set; }
    }
}