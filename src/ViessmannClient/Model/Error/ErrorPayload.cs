namespace PhilipDaubmeier.ViessmannClient.Model.Error
{
    public class ErrorPayload
    {
        public string? ViErrorId { get; set; }
        public int? StatusCode { get; set; }
        public string? ErrorType { get; set; }
        public string? Message { get; set; }
        public ExtendedErrorPayload? ExtendedPayload { get; set; }
    }
}