namespace PhilipDaubmeier.DigitalstromClient.Model.ZoneData
{
    public class LastCalledScenesResponse : IWiremessagePayload<LastCalledScenesResponse>
    {
        public int scene { get; set; }
    }
}