using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class Place
    {
        public double Altitude { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool ImproveLocProposed { get; set; }
        public List<double> Location { get; set; }
        public string Timezone { get; set; }
    }
}