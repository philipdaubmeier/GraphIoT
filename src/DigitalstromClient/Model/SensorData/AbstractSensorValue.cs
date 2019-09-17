using System;

namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public abstract class AbstractSensorValue
    {
        public double Value { get; set; }
        public DateTime Time { get; set; }
    }
}