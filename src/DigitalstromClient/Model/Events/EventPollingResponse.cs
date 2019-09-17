using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class EventPollingResponse : IWiremessagePayload
    {
        public List<DssEvent> Events { get; set; }
    }
}