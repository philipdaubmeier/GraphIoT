using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    internal class AuthProxyTokenResponse
    {
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } = string.Empty;

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}