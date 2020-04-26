using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    internal class TripStatisticsResponse : Wiremessage<Rts>
    {
        [JsonPropertyName("rtsViewModel")]
        public override Rts Body { get; set; } = new Rts();
    }
}