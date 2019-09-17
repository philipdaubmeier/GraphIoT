using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.NetatmoClient.Model.HomeData
{
    public class TimestampedMeasureCollection : IEnumerable<KeyValuePair<Measure, IEnumerable<KeyValuePair<DateTime, double>>>>
    {
        private readonly List<MeasureClump> _rawValues;
        private readonly List<Measure> _measures;

        internal TimestampedMeasureCollection(List<MeasureClump> rawValues, IEnumerable<Measure> measures)
        {
            _rawValues = rawValues;
            _measures = measures.ToList();
        }

        public IEnumerator<KeyValuePair<Measure, IEnumerable<KeyValuePair<DateTime, double>>>> GetEnumerator()
        {
            return GetTimestampedValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetTimestampedValues().GetEnumerator();
        }

        public IEnumerable<KeyValuePair<Measure, IEnumerable<KeyValuePair<DateTime, double>>>> GetTimestampedValues()
        {
            IEnumerable<KeyValuePair<DateTime, double>> GetMeasureValues(int index)
            {
                for (int n = 0; n < _rawValues.Count; n++)
                {
                    var clump = _rawValues[n];
                    var time = clump.BegTime;
                    for (int k = 0; k < clump.Value.Count; k++)
                    {
                        if (index < clump.Value[k].Count && clump.Value[k][index].HasValue)
                            yield return new KeyValuePair<DateTime, double>(time, clump.Value[k][index].Value);
                        time += clump.StepTime;
                    }
                }
            }

            if (_rawValues == null || _measures == null)
                yield break;

            for (int i = 0; i < _measures.Count; i++)
                yield return new KeyValuePair<Measure, IEnumerable<KeyValuePair<DateTime, double>>>(_measures[i], GetMeasureValues(i));
        }
    }
}