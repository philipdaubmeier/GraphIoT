using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class Event
    {
        public string Id { get; set; }
        public string Type { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Time { get; set; }

        public string CameraId { get; set; }
        public string VideoId { get; set; }
        public string VideoStatus { get; set; }
        public List<EventList> EventList { get; set; }
    }
}