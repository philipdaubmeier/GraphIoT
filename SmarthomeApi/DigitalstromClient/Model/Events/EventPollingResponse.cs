using System.Collections.Generic;

namespace DigitalstromClient.Model.Events
{
    public class EventPollingResponse : IWiremessagePayload<EventPollingResponse>
    {
        public List<DssEvent> events { get; set; }
    }
}
