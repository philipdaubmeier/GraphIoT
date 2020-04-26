using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class Home
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Place Place { get; set; } = new Place();
        public List<Camera> Cameras { get; set; } = new List<Camera>();
        public List<Event> Events { get; set; } = new List<Event>();
    }
}