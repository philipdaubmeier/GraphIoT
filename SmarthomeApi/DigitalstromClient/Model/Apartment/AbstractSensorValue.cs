using System;
using System.Globalization;

namespace DigitalstromClient.Model.Apartment
{
    public abstract class AbstractSensorValue
    {
        public double value { get; set; }

        private string _time;
        public string time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                DateTime.TryParseExact(_time, "yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _dateTime);
            }
        }

        private DateTime _dateTime;
        public DateTime dateTime { get { return _dateTime; } }

        public bool isValid()
        {
            return !string.IsNullOrWhiteSpace(_time);
        }
    }
}