using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class SensorValue
    {
        public Sensor Type { get; set; } = SensorType.UnknownType;
        public bool Valid { get; set; }
        public double Value { get; set; }
    }
}