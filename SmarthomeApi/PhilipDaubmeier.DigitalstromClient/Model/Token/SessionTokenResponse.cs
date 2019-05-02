namespace PhilipDaubmeier.DigitalstromClient.Model.Token
{
    public class SessionTokenResponse : IWiremessagePayload<SessionTokenResponse>
    {
        public string Token { get; set; }
    }
}