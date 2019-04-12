namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class CustomEventName : IEventName
    {
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
        }

        public CustomEventName(string name)
        {
            _name = name;
        }
    }
}