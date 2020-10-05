namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class Message
    {
        public string? Timestamp { get; set; }
        public string? Actor { get; set; }
        public string? ErrorCode { get; set; }
        public object? Status { get; set; }
        public string? @Event { get; set; }
        public string? StateMachine { get; set; }
        public int? Count { get; set; }
        public string? Priority { get; set; }
        public int? AdditionalInfo { get; set; }
    }
}