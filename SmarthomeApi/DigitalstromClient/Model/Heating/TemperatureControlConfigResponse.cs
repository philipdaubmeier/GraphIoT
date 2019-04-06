using System.Collections.Generic;

namespace DigitalstromClient.Model.Heating
{
    public class HeatingConfigZone
    {
        public int id { get; set; }
        public string name { get; set; }
        public string ControlDSUID { get; set; }
        public bool IsConfigured { get; set; }
        public int? ControlMode { get; set; }
        public int? EmergencyValue { get; set; }
        public double? CtrlKp { get; set; }
        public int? CtrlTs { get; set; }
        public int? CtrlTi { get; set; }
        public int? CtrlKd { get; set; }
        public double? CtrlImin { get; set; }
        public double? CtrlImax { get; set; }
        public int? CtrlYmin { get; set; }
        public int? CtrlYmax { get; set; }
        public bool? CtrlAntiWindUp { get; set; }
        public bool? CtrlKeepFloorWarm { get; set; }
        public int? ReferenceZone { get; set; }
        public int? CtrlOffset { get; set; }
    }

    public class TemperatureControlConfigResponse : IWiremessagePayload<TemperatureControlConfigResponse>
    {
        public List<HeatingConfigZone> zones { get; set; }
    }
}
