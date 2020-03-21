using System;

namespace PhilipDaubmeier.ViessmannClient.Model.Devices
{
    public class Device
    {
        public string? GatewaySerial { get; set; }
        public string? Id { get; set; }
        public string? BoilerSerial { get; set; }
        public string? BoilerSerialEditor { get; set; }
        public string? BmuSerial { get; set; }
        public string? BmuSerialEditor { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public string? ModelId { get; set; }
        public string? Status { get; set; }
        public string? DeviceType { get; set; }
    }
}