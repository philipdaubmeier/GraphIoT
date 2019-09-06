using Newtonsoft.Json;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class EventList
    {
        public string Type { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Time { get; set; }

        public int Offset { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
        public Snapshot Snapshot { get; set; }
        public Snapshot Vignette { get; set; }
    }
}