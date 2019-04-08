using System;
using System.Collections.Generic;
using DigitalstromClient.Model.Core;
using NodaTime;

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
        public DateTime timestamp { get { return Instant.FromUnixTimeSeconds(time).ToDateTimeUtc().ToLocalTime(); } }
    }
}