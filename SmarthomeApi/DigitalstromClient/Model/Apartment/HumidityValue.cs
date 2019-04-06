namespace DigitalstromClient.Model.Apartment
{
    public class HumidityValue : AbstractSensorValue
    {
        public static implicit operator HumidityValue(SensorValue sensor)
        {
            return new HumidityValue() { value = sensor.HumidityValue.GetValueOrDefault(0), time = sensor.HumidityValueTime };
        }
    }
}
