using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.WeatherStation
{
    public class WeatherStationData
    {
        public List<Device> Devices { get; set; }
        public User User { get; set; }
    }
}