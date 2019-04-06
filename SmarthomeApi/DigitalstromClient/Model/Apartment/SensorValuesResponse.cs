using System.Collections.Generic;

namespace DigitalstromClient.Model.Apartment
{
    public class SensorValuesResponse : IWiremessagePayload<SensorValuesResponse>
    {
        public Weather weather { get; set; }
        public Outdoor outdoor { get; set; }
        public List<Zone> zones { get; set; }
    }
}
