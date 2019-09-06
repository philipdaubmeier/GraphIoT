using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class TimestampedMeasureCollection
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime BegTime { get; set; }

        [JsonConverter(typeof(SecondsTimeSpanConverter))]
        public TimeSpan StepTime { get; set; }

        public List<List<double?>> Value { get; set; }
    }
}