using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Apartment
{
    public class Zone
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<SensorValue> values { get; set; }
        public TemperatureValue temperature { get { return values.Select(x => (TemperatureValue)x).FirstOrDefault(x => x.isValid()); } }
        public HumidityValue humidity { get { return values.Select(x => (HumidityValue)x).FirstOrDefault(x => x.isValid()); } }
        public CO2ConcentrationValue co2concentration { get { return values.Select(x => (CO2ConcentrationValue)x).FirstOrDefault(x => x.isValid()); } }
        public BrightnessValue brightness { get { return values.Select(x => (BrightnessValue)x).FirstOrDefault(x => x.isValid()); } }
    }
}