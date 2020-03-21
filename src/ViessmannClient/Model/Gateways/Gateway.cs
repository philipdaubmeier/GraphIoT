using System;

namespace PhilipDaubmeier.ViessmannClient.Model.Gateways
{
    public class Gateway
    {
        public long Id => long.TryParse(Serial, out long id) ? id : 0;
        public string? Serial { get; set; }
        public string? Version { get; set; }
        public int? FirmwareUpdateFailureCounter { get; set; }
        public bool? AutoUpdate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ProducedAt { get; set; }
        public DateTime? LastStatusChangedAt { get; set; }
        public string? AggregatedStatus { get; set; }
        public string? TargetRealm { get; set; }
        public string? GatewayType { get; set; }
        public int? InstallationId { get; set; }
        public DateTime? RegisteredAt { get; set; }
    }
}