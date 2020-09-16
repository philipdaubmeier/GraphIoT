namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class Message
    {
        public string? Timestamp { get; set; }
        public string? ErrorCode { get; set; }
        public string? Status { get; set; }
        public int? Count { get; set; }
        public string? Priority { get; set; }
    }
}