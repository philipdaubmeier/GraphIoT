using Newtonsoft.Json;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    internal class MeasureClump
    {
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime BegTime { get; set; }

        [JsonConverter(typeof(SecondsTimeSpanConverter))]
        public TimeSpan StepTime { get; set; }

        public List<List<double?>> Value { get; set; }

        public IEnumerable<KeyValuePair<Measure, IEnumerable<KeyValuePair<DateTime, double>>>> GetTimestampedValues(List<Measure> measures)
        {
            IEnumerable<KeyValuePair<DateTime, double>> GetMeasureValues(int index)
            {
                var time = BegTime;
                for (int k = 0; k < Value.Count; k++)
                {
                    if (index < Value[k].Count && Value[k][index].HasValue)
                        yield return new KeyValuePair<DateTime, double>(time, Value[k][index].Value);
                    time += StepTime;
                }
            }

            for (int i = 0; i < measures.Count; i++)
                yield return new KeyValuePair<Measure, IEnumerable<KeyValuePair<DateTime, double>>>(measures[i], GetMeasureValues(i));
        }
    }
}