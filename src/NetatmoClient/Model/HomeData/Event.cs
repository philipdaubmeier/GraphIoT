using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class Event
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Time { get; set; }

        public string CameraId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string VideoId { get; set; } = string.Empty;
        public string VideoStatus { get; set; } = string.Empty;
        public List<EventList> EventList { get; set; } = new List<EventList>();
    }
}