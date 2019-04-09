using System;

namespace DigitalstromClient.Model.Events
{
    public class DssEvent
    {
        public DateTime TimestampUtc { get; }
        public string name { get; set; }
        public DssEventProperties properties { get; set; }
        public DssEventSource source { get; set; }

        public SystemEventName systemEvent { get { return name; } }

        public DssEvent() : this(DateTime.UtcNow) { }

        public DssEvent(DateTime timestamp)
        {
            TimestampUtc = timestamp;
        }
    }
}