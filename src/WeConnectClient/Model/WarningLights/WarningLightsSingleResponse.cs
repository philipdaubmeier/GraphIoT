using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.WarningLights
{
    internal class WarningLightsSingleResponse : IWiremessage<WarningLightsEntry>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        [JsonPropertyName("data")]
        public virtual WarningLightsEntry Body { get; set; } = new();
    }
}