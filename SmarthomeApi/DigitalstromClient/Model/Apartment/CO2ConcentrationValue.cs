namespace DigitalstromClient.Model.Apartment
{
    public class CO2ConcentrationValue : AbstractSensorValue
    {
        public static implicit operator CO2ConcentrationValue(SensorValue sensor)
        {
            return new CO2ConcentrationValue() { value = sensor.CO2ConcentrationValue.GetValueOrDefault(0), time = sensor.CO2ConcentrationValueTime };
        }
    }
}
