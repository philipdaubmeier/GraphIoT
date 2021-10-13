using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    internal class TripStatisticsListResponse : IWiremessage<List<TripStatisticEntry>>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        [JsonPropertyName("data")]
        public virtual List<TripStatisticEntry> Body { get; set; } = new();
    }
}