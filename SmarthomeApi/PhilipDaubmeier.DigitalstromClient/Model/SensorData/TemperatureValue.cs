namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class TemperatureValue : AbstractSensorValue
    {
        public static implicit operator TemperatureValue(SensorValue sensor)
        {
            return sensor.TemperatureValueTime.HasValue ? new TemperatureValue()
            {
                Value = sensor.TemperatureValue.GetValueOrDefault(0),
                Time = sensor.TemperatureValueTime.Value
            } : null;
        }
    }
}
