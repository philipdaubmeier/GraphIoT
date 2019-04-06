namespace DigitalstromClient.Model.Auth
{
    public class SessionTokenResponse : IWiremessagePayload<SessionTokenResponse>
    {
        public string token { get; set; }
    }
}
