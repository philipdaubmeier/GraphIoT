using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class Home
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Place Place { get; set; }
        public List<Camera> Cameras { get; set; }
        public List<Event> Events { get; set; }
    }
}