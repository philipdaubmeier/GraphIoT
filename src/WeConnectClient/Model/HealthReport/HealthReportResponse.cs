using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.HealthReport
{
    internal class HealthReportResponse : Wiremessage<List<HealthReport>>
    {
        [JsonPropertyName("vehicleHealthReportList")]
        public override List<HealthReport> Body { get; set; } = new List<HealthReport>();

        [JsonPropertyName("X-RateLimit-CreationTask-Reset")]
        public string RateLimitReset { get; set; } = string.Empty;

        [JsonPropertyName("X-RateLimit-CreationTask-Limit")]
        public string RateLimitMax { get; set; } = string.Empty;

        [JsonPropertyName("X-RateLimit-CreationTask-Remaining")]
        public string RateLimitRemaining { get; set; } = string.Empty;
    }
}