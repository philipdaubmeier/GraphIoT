using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class HomeData
    {
        public List<Home> Homes { get; set; }
        public User User { get; set; }
        public GlobalInfo GlobalInfo { get; set; }
    }
}