namespace DigitalstromClient.Model.Apartment
{
    public class SensorValue
    {
        public double? TemperatureValue { get; set; }
        public string TemperatureValueTime { get; set; }
        public double? HumidityValue { get; set; }
        public string HumidityValueTime { get; set; }
        public double? CO2ConcentrationValue { get; set; }
        public string CO2ConcentrationValueTime { get; set; }
        public double? BrightnessValue { get; set; }
        public string BrightnessValueTime { get; set; }
    }
}