namespace PhilipDaubmeier.ViessmannClient.Model.Error
{
    public class ExtendedErrorPayload
    {
        public string? Name { get; set; }
        public long? RequestCountLimit { get; set; }
        public string? ClientId { get; set; }
        public string? UserId { get; set; }
        public long? LimitReset { get; set; }
    }
}