using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.SensorData
{
    public class SensorValuesResponse : IWiremessagePayload<SensorValuesResponse>
    {
        public Weather Weather { get; set; }
        public Outdoor Outdoor { get; set; }
        public List<ZoneSensorValues> Zones { get; set; }
    }
}