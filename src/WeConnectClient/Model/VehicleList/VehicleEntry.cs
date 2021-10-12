using PhilipDaubmeier.WeConnectClient.Model.Core;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleList
{
    public class VehicleEntry
    {
        public Vin Vin => Vehicle.Vin ?? Vin.Empty;

        public string VehicleNickname { get; set; } = string.Empty;
        public bool RoleReseted { get; set; } = false;
        public string LicensePlate { get; set; } = string.Empty;
        public string DealerId { get; set; } = string.Empty;
        public string DealerBrandCode { get; set; } = string.Empty;
        public string AllocatedDealerCountry { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public VehicleIdDetail Vehicle { get; set; } = new();
    }

    public class VehicleIdDetail
    {
        [JsonConverter(typeof(VinConverter))]
        public Vin? Vin { get; set; } = null;
        public string CommissionId { get; set; } = string.Empty;
        public string ModBackend { get; set; } = string.Empty;
    }
}