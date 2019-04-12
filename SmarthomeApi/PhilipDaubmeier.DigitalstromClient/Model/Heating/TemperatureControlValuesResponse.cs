using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Heating
{
    public class HeatingValuesZone
    {
        public int id { get; set; }
        public string name { get; set; }
        public string ControlDSUID { get; set; }
        public bool IsConfigured { get; set; }
        public double? Off { get; set; }
        public double? Comfort { get; set; }
        public double? Economy { get; set; }
        public double? NotUsed { get; set; }
        public double? Night { get; set; }
        public double? Holiday { get; set; }
        public double? Cooling { get; set; }
        public double? CoolingOff { get; set; }
    }

    public class TemperatureControlValuesResponse : IWiremessagePayload<TemperatureControlValuesResponse>
    {
        public List<HeatingValuesZone> zones { get; set; }
    }
}
