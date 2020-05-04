using System;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class Charger
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string ChargePointId { get; set; } = string.Empty;
        public string DefaultChargingMode { get; set; } = string.Empty;
        public string DefaultDepartureTime { get; set; } = string.Empty;
        public string ChargingAuthorizationType { get; set; } = string.Empty;
        public string ChargingUnitType { get; set; } = string.Empty;
        public bool SmartmodePaused { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public string WebsocketToken { get; set; } = string.Empty;
    }
}