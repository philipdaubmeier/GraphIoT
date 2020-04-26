using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    internal class LoginPageInfoResponse : Wiremessage<LoginUrl>
    {
        [JsonPropertyName("loginURL")]
        public override LoginUrl Body { get; set; } = new LoginUrl();
    }

    internal class LoginUrl
    {
        public string Path { get; set; } = string.Empty;
    }
}