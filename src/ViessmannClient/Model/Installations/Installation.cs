using System;

namespace PhilipDaubmeier.ViessmannClient.Model.Installations
{
    public class Installation
    {
        public long LongId => Id ?? 0;
        public int? Id { get; set; }
        public string? Description { get; set; }
        public Address? Address { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AggregatedStatus { get; set; }
        public string? ServicedBy { get; set; }
        public string? HeatingType { get; set; }
    }
}