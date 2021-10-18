using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.WarningLights
{
    internal class WarningLightsListResponse : IWiremessage<List<WarningLightsEntry>>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        [JsonPropertyName("data")]
        public virtual List<WarningLightsEntry> Body { get; set; } = new();
    }
}