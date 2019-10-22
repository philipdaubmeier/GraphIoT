using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Heating
{
    public class HeatingStatusZone
    {
        public Zone Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public int ControlMode { get; set; }
        public int ControlState { get; set; }
        public Dsuid ControlDSUID { get; set; } = string.Empty;
        public bool IsConfigured { get; set; }
        public int? OperationMode { get; set; }
        public double? TemperatureValue { get; set; }
        public DateTime? TemperatureValueTime { get; set; }
        public double? NominalValue { get; set; }
        public DateTime? NominalValueTime { get; set; }
        public double? ControlValue { get; set; }
        public DateTime? ControlValueTime { get; set; }
    }

    public class TemperatureControlStatusResponse : IWiremessagePayload
    {
        public List<HeatingStatusZone> Zones { get; set; } = new List<HeatingStatusZone>();
    }
}