namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class CO2ConcentrationValue : AbstractSensorValue
    {
        public static implicit operator CO2ConcentrationValue?(SensorValue sensor)
        {
            return sensor.CO2ConcentrationValueTime.HasValue ? new CO2ConcentrationValue()
            {
                Value = sensor.CO2ConcentrationValue.GetValueOrDefault(0),
                Time = sensor.CO2ConcentrationValueTime.Value
            } : null;
        }
    }
}