namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class BrightnessValue : AbstractSensorValue
    {
        public static implicit operator BrightnessValue(SensorValue sensor)
        {
            return sensor.BrightnessValueTime.HasValue ? new BrightnessValue()
            {
                Value = sensor.BrightnessValue.GetValueOrDefault(0),
                Time = sensor.BrightnessValueTime.Value
            } : null;
        }
    }
}
