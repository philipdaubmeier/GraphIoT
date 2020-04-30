using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleList
{
    internal class LoadCarDetailsResponse : Wiremessage<VehicleEntry>
    {
        [JsonPropertyName("completeVehicleJson")]
        public override VehicleEntry Body { get; set; } = new VehicleEntry();
    }
}