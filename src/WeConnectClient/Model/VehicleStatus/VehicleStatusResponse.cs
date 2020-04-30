using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleStatus
{
    internal class VehicleStatusResponse : Wiremessage<VehicleStatus>
    {
        [JsonPropertyName("vehicleStatusData")]
        public override VehicleStatus Body { get; set; } = new VehicleStatus();
    }
}