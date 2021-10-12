using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleList
{
    internal class VehicleListResponse : IWiremessage<List<VehicleEntry>>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        [JsonPropertyName("relations")]
        public List<VehicleEntry> Body { get; set; } = new List<VehicleEntry>();
    }
}