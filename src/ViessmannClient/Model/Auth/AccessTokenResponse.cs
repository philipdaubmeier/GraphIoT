namespace PhilipDaubmeier.ViessmannClient.Model.Auth
{
    public class AccessTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? ExpiresIn { get; set; }
        public string? TokenType { get; set; }
    }
}