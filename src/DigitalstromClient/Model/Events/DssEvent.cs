using System;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class DssEvent
    {
        public DateTime TimestampUtc { get; }
        public string Name { get; set; }
        public DssEventProperties Properties { get; set; }
        public DssEventSource Source { get; set; }

        public SystemEventName SystemEvent => Name;

        public DssEvent() : this(DateTime.UtcNow) { }

        public DssEvent(DateTime timestamp)
        {
            TimestampUtc = timestamp;
        }
    }
}