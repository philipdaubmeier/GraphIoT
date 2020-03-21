using Newtonsoft.Json;

namespace PhilipDaubmeier.ViessmannClient.Model.Auth
{
    public class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public string? ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }
    }
}