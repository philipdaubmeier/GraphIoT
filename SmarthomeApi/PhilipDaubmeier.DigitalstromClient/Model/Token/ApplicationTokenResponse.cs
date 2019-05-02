namespace PhilipDaubmeier.DigitalstromClient.Model.Token
{
    public class ApplicationTokenResponse : IWiremessagePayload<ApplicationTokenResponse>
    {
        public string ApplicationToken { get; set; }
    }
}
