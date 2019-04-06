namespace DigitalstromClient.Model.Events
{
    public class DssEvent
    {
        public string name { get; set; }
        public DssEventProperties properties { get; set; }
        public DssEventSource source { get; set; }

        public SystemEventName systemEvent { get { return name; } }
    }
}
