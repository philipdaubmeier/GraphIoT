using System;
using System.Collections.Generic;
using DigitalstromClient.Model.Core;

namespace DigitalstromClient.Model.PropertyTree
{
    public class ZonesAndSensorValuesResponse : IWiremessagePayload<ZonesAndSensorValuesResponse>
    {
        public List<ZonesAndSensorValues> zones { get; set; }
    }

    public class ZonesAndSensorValues
    {
        public int ZoneID { get; set; }
        public List<SensorTypeAndValues> sensor { get; set; }
    }
    
    public class SensorTypeAndValues
    {
        public int type { get; set; }
        public double value { get; set; }
        public long time { get; set; }

        public SensorType sensorType { get { return type; } }
        public DateTime timestamp { get { return epoch.AddSeconds(time).ToLocalTime(); } }
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}