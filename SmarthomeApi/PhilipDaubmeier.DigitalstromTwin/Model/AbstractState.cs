using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public abstract class AbstractState<T> : INotifyPropertyChanged
    {
        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!(_propertyChanged?.GetInvocationList()?.Contains(value) ?? false))
                    _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        private PropertyChangedEventHandler _propertyChangedInternal;
        internal event PropertyChangedEventHandler PropertyChangedInternal
        {
            add
            {
                if (!(_propertyChangedInternal?.GetInvocationList()?.Contains(value) ?? false))
                    _propertyChangedInternal += value;
            }
            remove
            {
                _propertyChangedInternal -= value;
            }
        }

        public DateTime Timestamp { get; set; }

        private Semaphore _stateSemaphore = new Semaphore(1, 1);

        private T _value;
        public T Value
        {
            get
            {
                _stateSemaphore.WaitOne();
                try
                {
                    return _value;
                }
                finally
                {
                    _stateSemaphore.Release();
                }
            }
            set
            {
                _stateSemaphore.WaitOne();
                try
                {
                    _value = value;
                    Timestamp = DateTime.UtcNow;
                }
                finally
                {
                    _stateSemaphore.Release();
                }
                NotifyChanged();
            }
        }

        internal T ValueInternal
        {
            get
            {
                _stateSemaphore.WaitOne();
                try
                {
                    return _value;
                }
                finally
                {
                    _stateSemaphore.Release();
                }
            }
            set
            {
                _stateSemaphore.WaitOne();
                try
                {
                    _value = value;
                    Timestamp = DateTime.UtcNow;
                }
                finally
                {
                    _stateSemaphore.Release();
                }
                NotifyChanged(false);
            }
        }

        private void NotifyChanged(bool useInternal = true)
        {
            var propertyChangedInternalHandler = _propertyChangedInternal;
            if (useInternal && propertyChangedInternalHandler != null)
                propertyChangedInternalHandler(this, new PropertyChangedEventArgs(nameof(Value)));

            var propertyChangedHandler = _propertyChanged;
            if (propertyChangedHandler == null)
                return;

            propertyChangedHandler(this, new PropertyChangedEventArgs(nameof(Value)));
            propertyChangedHandler(this, new PropertyChangedEventArgs(nameof(Timestamp)));
        }
    }
}