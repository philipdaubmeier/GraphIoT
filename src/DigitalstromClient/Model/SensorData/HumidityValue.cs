namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class HumidityValue : AbstractSensorValue
    {
        public static implicit operator HumidityValue(SensorValue sensor)
        {
            return sensor.HumidityValueTime.HasValue ? new HumidityValue()
            {
                Value = sensor.HumidityValue.GetValueOrDefault(0),
                Time = sensor.HumidityValueTime.Value
            } : null;
        }
    }
}
