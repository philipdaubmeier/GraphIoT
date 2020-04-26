using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    public class RtsConfiguration
    {
        [JsonPropertyName("electric_consumption")]
        public bool ElectricConsumption { get; set; }

        [JsonPropertyName("triptype_short")]
        public bool TriptypeShort { get; set; }

        [JsonPropertyName("auxiliary_consumption")]
        public bool AuxiliaryConsumption { get; set; }

        [JsonPropertyName("fuel_overall_consumption")]
        public bool FuelOverallConsumption { get; set; }

        [JsonPropertyName("triptype_cyclic")]
        public bool TriptypeCyclic { get; set; }

        [JsonPropertyName("electric_overall_consumption")]
        public bool ElectricOverallConsumption { get; set; }

        [JsonPropertyName("triptype_long")]
        public bool TriptypeLong { get; set; }

        [JsonPropertyName("cng_overall_consumption")]
        public bool CngOverallConsumption { get; set; }

        public bool Recuperation { get; set; }
    }
}