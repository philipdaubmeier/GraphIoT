namespace PhilipDaubmeier.DigitalstromClient.Model.Auth
{
    public class ApplicationTokenResponse : IWiremessagePayload<ApplicationTokenResponse>
    {
        public string applicationToken { get; set; }
    }
}
