using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class SensorValuesResponse : IWiremessagePayload
    {
        public Weather Weather { get; set; } = new Weather();
        public Outdoor Outdoor { get; set; } = new Outdoor();
        public List<ZoneSensorValues> Zones { get; set; } = new List<ZoneSensorValues>();
    }
}