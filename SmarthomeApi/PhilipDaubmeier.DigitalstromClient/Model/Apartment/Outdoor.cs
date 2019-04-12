namespace PhilipDaubmeier.DigitalstromClient.Model.Apartment
{
    public class Outdoor
    {
        public TemperatureValue temperature { get; set; }
        public HumidityValue humidity { get; set; }
        public BrightnessValue brightness { get; set; }
        public PrecipitationValue precipitation { get; set; }
        public AirPressureValue airpressure { get; set; }
        public WindSpeedValue windspeed { get; set; }
        public WindDirectionValue winddirection { get; set; }
        public GustSpeedValue gustspeed { get; set; }
        public GustDirectionValue gustdirection { get; set; }
    }

    public class PrecipitationValue : AbstractSensorValue { }
    public class AirPressureValue : AbstractSensorValue { }
    public class WindSpeedValue : AbstractSensorValue { }
    public class WindDirectionValue : AbstractSensorValue { }
    public class GustSpeedValue : AbstractSensorValue { }
    public class GustDirectionValue : AbstractSensorValue { }
}
