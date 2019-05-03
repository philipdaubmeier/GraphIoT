using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class Device
    {
        public string Id { get; set; }
        public string DisplayID { get; set; }
        public Dsuid DSUID { get; set; }
        public string GTIN { get; set; }
        public string Name { get; set; }
        public int DSUIDIndex { get; set; }
        public int FunctionID { get; set; }
        public int ProductRevision { get; set; }
        public int ProductID { get; set; }
        public string HwInfo { get; set; }
        public string OemStatus { get; set; }
        public string OemEanNumber { get; set; }
        public int OemSerialNumber { get; set; }
        public int OemPartNumber { get; set; }
        public string OemProductInfoState { get; set; }
        public string OemProductURL { get; set; }
        public string OemInternetState { get; set; }
        public bool OemIsIndependent { get; set; }
        public ModelFeatures ModelFeatures { get; set; }
        public bool IsVdcDevice { get; set; }
        public string MeterDSID { get; set; }
        public Dsuid MeterDSUID { get; set; }
        public string MeterName { get; set; }
        public int BusID { get; set; }
        public Zone ZoneID { get; set; }
        public bool IsPresent { get; set; }
        public bool IsValid { get; set; }
        public DateTime LastDiscovered { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime InactiveSince { get; set; }
        public bool On { get; set; }
        public bool Locked { get; set; }
        public bool ConfigurationLocked { get; set; }
        public int OutputMode { get; set; }
        public int ButtonID { get; set; }
        public int ButtonActiveGroup { get; set; }
        public int ButtonGroupMembership { get; set; }
        public int ButtonInputMode { get; set; }
        public int ButtonInputIndex { get; set; }
        public int ButtonInputCount { get; set; }
        public string AKMInputProperty { get; set; }
        public List<Group> Groups { get; set; }
        public int BinaryInputCount { get; set; }
        public List<BinaryInput> BinaryInputs { get; set; }
        public int SensorInputCount { get; set; }
        public List<SensorValue> Sensors { get; set; }
        public bool SensorDataValid { get; set; }
        public List<object> OutputChannels { get; set; }
        public List<string> PairedDevices { get; set; }
        public string ValveType { get; set; }
    }
}