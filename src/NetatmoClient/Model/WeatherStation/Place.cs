using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class Place
    {
        public double Altitude { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool ImproveLocProposed { get; set; }
        public List<double> Location { get; set; } = new List<double>();
        public string Timezone { get; set; } = string.Empty;
    }
}