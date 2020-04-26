using System;

namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class SensorValue
    {
        public double? TemperatureValue { get; set; }
        public DateTime? TemperatureValueTime { get; set; }
        public double? HumidityValue { get; set; }
        public DateTime? HumidityValueTime { get; set; }
        public double? CO2ConcentrationValue { get; set; }
        public DateTime? CO2ConcentrationValueTime { get; set; }
        public double? BrightnessValue { get; set; }
        public DateTime? BrightnessValueTime { get; set; }
    }
}