using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    internal class TripStatisticsSingleResponse : IWiremessage<TripStatisticEntry>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        [JsonPropertyName("data")]
        public virtual TripStatisticEntry Body { get; set; } = new();
    }
}