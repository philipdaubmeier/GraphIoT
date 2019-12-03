using Newtonsoft.Json;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class EventList
    {
        public string Type { get; set; } = string.Empty;

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Time { get; set; }

        public int Offset { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Snapshot Snapshot { get; set; } = new Snapshot();
        public Snapshot Vignette { get; set; } = new Snapshot();
    }
}