using System;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class DssEvent
    {
        public DateTime TimestampUtc { get; }
        public string Name { get; set; } = string.Empty;
        public DssEventProperties Properties { get; set; } = new DssEventProperties();
        public DssEventSource Source { get; set; } = new DssEventSource();

        public SystemEventName SystemEvent => Name;

        public DssEvent() : this(DateTime.UtcNow) { }

        public DssEvent(DateTime timestamp)
        {
            TimestampUtc = timestamp;
        }
    }
}