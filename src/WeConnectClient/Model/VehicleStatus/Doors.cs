using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleStatus
{
    public class Doors
    {
        [JsonPropertyName("left_front")]
        public int LeftFront { get; set; }

        [JsonPropertyName("right_front")]
        public int RightFront { get; set; }

        [JsonPropertyName("left_back")]
        public int? LeftBack { get; set; }

        [JsonPropertyName("right_back")]
        public int? RightBack { get; set; }

        public int? Trunk { get; set; }

        [JsonPropertyName("number_of_doors")]
        public int? NumberOfDoors { get; set; }
    }
}