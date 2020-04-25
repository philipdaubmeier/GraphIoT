using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    internal class LoginPageInfoResponse
    {
        public int ErrorCode { get; set; } = 0;

        [JsonPropertyName("loginURL")]
        public LoginUrl? LoginUrl { get; set; }
    }

    internal class LoginUrl
    {
        public string? Path { get; set; }
    }
}