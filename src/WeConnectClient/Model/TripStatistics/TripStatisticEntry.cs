using System;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    public class TripStatisticEntry
    {
        public string Id { get; set; } = string.Empty;
        public DateTime TripEndTimestamp { get; set; } = DateTime.MinValue;
        public string TripType { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;

        [JsonPropertyName("mileage_km")]
        public double MileageKm { get; set; } = 0;

        [JsonPropertyName("startMileage_km")]
        public double StartMileageKm { get; set; } = 0;

        [JsonPropertyName("overallMileage_km")]
        public double OverallMileageKm { get; set; } = 0;
        public double TravelTime { get; set; } = 0;

        public double? AverageFuelConsumption { get; set; }
        public double? AverageElectricConsumption { get; set; }
        public double? AverageGasConsumption { get; set; }
        public double? AverageAuxConsumption { get; set; }
        public double? AverageRecuperation { get; set; }

        [JsonPropertyName("averageSpeed_kmph")]
        public double AverageSpeedKmph { get; set; } = 0d;
    }
}