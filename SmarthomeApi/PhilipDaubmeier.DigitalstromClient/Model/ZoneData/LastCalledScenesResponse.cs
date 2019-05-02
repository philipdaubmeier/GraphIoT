using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.ZoneData
{
    public class LastCalledScenesResponse : IWiremessagePayload<LastCalledScenesResponse>
    {
        public Scene Scene { get; set; }
    }
}