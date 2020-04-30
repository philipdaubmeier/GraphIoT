using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleInfo
{
    internal class VehicleDetailsResponse : Wiremessage<VehicleDetails>
    {
        [JsonPropertyName("vehicleDetails")]
        public override VehicleDetails Body { get; set; } = new VehicleDetails();
    }
}