using System;
using System.Collections.Generic;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using NodaTime;

namespace PhilipDaubmeier.DigitalstromClient.Model.PropertyTree
{
    public class ZonesAndSensorValuesResponse : IWiremessagePayload
    {
        public List<ZonesAndSensorValues> Zones { get; set; }
    }

    public class ZonesAndSensorValues
    {
        public Zone ZoneID { get; set; }
        public List<SensorTypeAndValues> Sensor { get; set; }
    }
    
    public class SensorTypeAndValues
    {
        public Sensor Type { get; set; }
        public double Value { get; set; }
        public long Time { get; set; }

        public DateTime Timestamp { get { return Instant.FromUnixTimeSeconds(Time).ToDateTimeUtc().ToLocalTime(); } }
    }
}