using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class HomeData
    {
        public List<Home> Homes { get; set; } = new List<Home>();
        public User User { get; set; } = new User();
        public GlobalInfo GlobalInfo { get; set; } = new GlobalInfo();
    }
}