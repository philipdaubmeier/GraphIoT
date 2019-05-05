using System;
using System.ComponentModel;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public abstract class AbstractState<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal event PropertyChangedEventHandler PropertyChangedInternal;

        public DateTime Timestamp { get; set; }

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Timestamp = DateTime.UtcNow;
                NotifyChanged();
            }
        }

        internal T ValueInternal
        {
            get { return _value; }
            set
            {
                _value = value;
                Timestamp = DateTime.UtcNow;
                NotifyChanged(false);
            }
        }

        private void NotifyChanged(bool useInternal = true)
        {
            var propertyChangedInternalHandler = PropertyChangedInternal;
            if (useInternal && propertyChangedInternalHandler != null)
            {
                propertyChangedInternalHandler(this, new PropertyChangedEventArgs(nameof(Value)));
                return;
            }

            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler == null)
                return;

            propertyChangedHandler(this, new PropertyChangedEventArgs(nameof(Value)));
            propertyChangedHandler(this, new PropertyChangedEventArgs(nameof(Timestamp)));
        }
    }
}