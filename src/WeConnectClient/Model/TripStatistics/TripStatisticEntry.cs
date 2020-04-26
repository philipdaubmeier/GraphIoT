namespace PhilipDaubmeier.WeConnectClient.Model.TripStatistics
{
    public class TripStatisticEntry
    {
        public int TripId { get; set; }
        public double? AverageElectricConsumption { get; set; }
        public double? AverageFuelConsumption { get; set; }
        public double? AverageCngConsumption { get; set; }
        public double AverageSpeed { get; set; }
        public double TripDuration { get; set; }
        public double TripLength { get; set; }
        public string Timestamp { get; set; } = string.Empty;
        public string TripDurationFormatted { get; set; } = string.Empty;
        public double? Recuperation { get; set; }
        public double? AverageAuxiliaryConsumption { get; set; }
        public double? TotalElectricConsumption { get; set; }
        public string LongFormattedTimestamp { get; set; } = string.Empty;
    }
}