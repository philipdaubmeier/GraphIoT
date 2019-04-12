using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class EventPollingResponse : IWiremessagePayload<EventPollingResponse>
    {
        public List<DssEvent> events { get; set; }
    }
}
