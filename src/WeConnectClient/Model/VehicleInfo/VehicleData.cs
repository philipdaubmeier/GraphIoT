using PhilipDaubmeier.WeConnectClient.Model.Core;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleInfo
{
    public class VehicleData
    {
        [JsonConverter(typeof(VinConverter))]
        public Vin? Vin { get; set; } = null;
        public string ModelName { get; set; } = string.Empty;
        public string ExteriorColor { get; set; } = string.Empty;
    }
}