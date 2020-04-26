namespace PhilipDaubmeier.SonnenClient.Model
{
    public class AccessTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public int CreatedAt { get; set; }
    }
}