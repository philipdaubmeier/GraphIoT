using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class Device
    {
        public string id { get; set; }
        public string DisplayID { get; set; }
        public string dSUID { get; set; }
        public string GTIN { get; set; }
        public string name { get; set; }
        public int dSUIDIndex { get; set; }
        public int functionID { get; set; }
        public int productRevision { get; set; }
        public int productID { get; set; }
        public string hwInfo { get; set; }
        public string OemStatus { get; set; }
        public string OemEanNumber { get; set; }
        public int OemSerialNumber { get; set; }
        public int OemPartNumber { get; set; }
        public string OemProductInfoState { get; set; }
        public string OemProductURL { get; set; }
        public string OemInternetState { get; set; }
        public bool OemIsIndependent { get; set; }
        public ModelFeatures modelFeatures { get; set; }
        public bool isVdcDevice { get; set; }
        public string meterDSID { get; set; }
        public string meterDSUID { get; set; }
        public string meterName { get; set; }
        public int busID { get; set; }
        public int zoneID { get; set; }
        public bool isPresent { get; set; }
        public bool isValid { get; set; }
        public string lastDiscovered { get; set; }
        public string firstSeen { get; set; }
        public string inactiveSince { get; set; }
        public bool on { get; set; }
        public bool locked { get; set; }
        public bool configurationLocked { get; set; }
        public int outputMode { get; set; }
        public int buttonID { get; set; }
        public int buttonActiveGroup { get; set; }
        public int buttonGroupMembership { get; set; }
        public int buttonInputMode { get; set; }
        public int buttonInputIndex { get; set; }
        public int buttonInputCount { get; set; }
        public string AKMInputProperty { get; set; }
        public List<int> groups { get; set; }
        public int binaryInputCount { get; set; }
        public List<BinaryInput> binaryInputs { get; set; }
        public int sensorInputCount { get; set; }
        public List<Sensor> sensors { get; set; }
        public bool sensorDataValid { get; set; }
        public List<object> outputChannels { get; set; }
        public List<string> pairedDevices { get; set; }
        public string ValveType { get; set; }
    }
}
