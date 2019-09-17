namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class Outdoor
    {
        public TemperatureValue Temperature { get; set; }
        public HumidityValue Humidity { get; set; }
        public BrightnessValue Brightness { get; set; }
        public PrecipitationValue Precipitation { get; set; }
        public AirPressureValue Airpressure { get; set; }
        public WindSpeedValue Windspeed { get; set; }
        public WindDirectionValue Winddirection { get; set; }
        public GustSpeedValue Gustspeed { get; set; }
        public GustDirectionValue Gustdirection { get; set; }
    }

    public class PrecipitationValue : AbstractSensorValue { }
    public class AirPressureValue : AbstractSensorValue { }
    public class WindSpeedValue : AbstractSensorValue { }
    public class WindDirectionValue : AbstractSensorValue { }
    public class GustSpeedValue : AbstractSensorValue { }
    public class GustDirectionValue : AbstractSensorValue { }
}