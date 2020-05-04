using System;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class Car
    {
        [JsonPropertyName("consumption_kwh_100km")]
        public double ConsumptionKwh100km { get; set; }

        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public bool Active { get; set; }
        public double? CapacityKwh { get; set; }
        public double? ChargedEnergy { get; set; }
        public double? ChargedKm { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}