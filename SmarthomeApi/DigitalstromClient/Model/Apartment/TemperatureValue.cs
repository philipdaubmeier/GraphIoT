namespace DigitalstromClient.Model.Apartment
{
    public class TemperatureValue : AbstractSensorValue
    {
        public static implicit operator TemperatureValue(SensorValue sensor)
        {
            return new TemperatureValue() { value = sensor.TemperatureValue.GetValueOrDefault(0), time = sensor.TemperatureValueTime };
        }
    }
}
