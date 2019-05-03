using System;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public abstract class AbstractState<T>
    {
        public DateTime Timestamp { get; set; }

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Timestamp = DateTime.UtcNow;
            }
        }
    }
}