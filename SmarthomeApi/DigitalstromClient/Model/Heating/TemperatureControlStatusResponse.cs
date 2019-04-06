using System.Collections.Generic;

namespace DigitalstromClient.Model.Heating
{
    public class HeatingStatusZone
    {
        public int id { get; set; }
        public string name { get; set; }
        public int ControlMode { get; set; }
        public int ControlState { get; set; }
        public string ControlDSUID { get; set; }
        public bool IsConfigured { get; set; }
        public int? OperationMode { get; set; }
        public double? TemperatureValue { get; set; }
        public string TemperatureValueTime { get; set; }
        public double? NominalValue { get; set; }
        public string NominalValueTime { get; set; }
        public double? ControlValue { get; set; }
        public string ControlValueTime { get; set; }
    }

    public class TemperatureControlStatusResponse : IWiremessagePayload<TemperatureControlStatusResponse>
    {
        public List<HeatingStatusZone> zones { get; set; }
    }
}
