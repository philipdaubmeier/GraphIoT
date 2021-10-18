using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Fuel
{
    internal class FuelStatusResponse : IWiremessage<List<FuelStatus>>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        [JsonPropertyName("data")]
        public virtual List<FuelStatus> Body { get; set; } = new();
    }
}