using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class ZonesAndSensorValuesResponse : IWiremessagePayload
    {
        public List<ZonesAndSensorValues> Zones { get; set; } = new List<ZonesAndSensorValues>();
    }

    public class ZonesAndSensorValues
    {
        public Zone ZoneID { get; set; } = 0;
        public List<SensorTypeAndValues> Sensor { get; set; } = new List<SensorTypeAndValues>();
    }

    public class SensorTypeAndValues
    {
        public Sensor Type { get; set; } = SensorType.UnknownType;
        public double Value { get; set; }
        public long Time { get; set; }

        public DateTime Timestamp { get { return DateTimeOffset.FromUnixTimeSeconds(Time).UtcDateTime.ToLocalTime(); } }
    }
}