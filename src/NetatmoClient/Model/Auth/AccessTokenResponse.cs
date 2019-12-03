namespace PhilipDaubmeier.NetatmoClient.Model.Auth
{
    public class AccessTokenResponse
    {
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
    }
}