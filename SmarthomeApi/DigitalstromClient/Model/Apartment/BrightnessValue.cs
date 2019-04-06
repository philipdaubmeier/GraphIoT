namespace DigitalstromClient.Model.Apartment
{
    public class BrightnessValue : AbstractSensorValue
    {
        public static implicit operator BrightnessValue(SensorValue sensor)
        {
            return new BrightnessValue() { value = sensor.BrightnessValue.GetValueOrDefault(0), time = sensor.BrightnessValueTime };
        }
    }
}
