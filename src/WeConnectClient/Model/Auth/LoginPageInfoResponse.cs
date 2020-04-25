using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    internal class LoginPageInfoResponse
    {
        public string ErrorCode { get; set; } = string.Empty;

        [JsonPropertyName("loginURL")]
        public LoginUrl? LoginUrl { get; set; }
    }

    internal class LoginUrl
    {
        public string? Path { get; set; }
    }
}