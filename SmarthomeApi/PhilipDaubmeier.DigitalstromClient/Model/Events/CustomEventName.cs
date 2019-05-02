namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class CustomEventName : IEventName
    {
        public string Name { get; }

        public CustomEventName(string name)
        {
            Name = name;
        }
    }
}