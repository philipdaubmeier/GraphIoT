using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactTimeSeries
{
    public class TimeSeriesResampler<Tseries, Tval> where Tval : struct where Tseries : TimeSeriesBase<Tval>
    {
        public ITimeSeries<Tval> Resampled { get; private set; }

        public TimeSeriesResampler(TimeSeriesSpan span)
        {
            Resampled = (ITimeSeries<Tval>)Activator.CreateInstance(typeof(Tseries), span);
        }

        public void SampleAccumulate(ITimeSeries<Tval> timeseries)
        {
            foreach (var item in timeseries)
                if (item.Value.HasValue)
                    Resampled.Accumulate(item.Key, item.Value.Value);
        }

        public void SampleAccumulate(IEnumerable<ITimeSeries<Tval>> timeseries)
        {
            foreach (var series in timeseries)
                SampleAccumulate(series);
        }

        public void SampleAverage(ITimeSeries<Tval> timeseries, Func<Tval, decimal> selector, Func<decimal, Tval> resultCast)
        {
            SampleAggregate(timeseries, x => resultCast(x.Average(selector)));
        }

        public void SampleAggregate(ITimeSeries<Tval> timeseries, Func<IEnumerable<Tval>, Tval> aggregate)
        {
            IEnumerator<KeyValuePair<DateTime, Tval?>> enumerator = Resampled.GetEnumerator();
            DateTime? lastTimeBucket = null;
            List<Tval> values = new List<Tval>();
            bool breakOuter = false;

            foreach (var item in timeseries)
            {
                while (enumerator.Current.Key > item.Key || (enumerator.Current.Key + Resampled.Span.Duration) <= item.Key)
                {
                    if (!enumerator.MoveNext())
                    {
                        breakOuter = true;
                        break;
                    }
                }

                if (!lastTimeBucket.HasValue)
                    lastTimeBucket = enumerator.Current.Key;

                if (breakOuter || lastTimeBucket.Value != enumerator.Current.Key)
                {
                    if (values.Count > 0)
                        Resampled[lastTimeBucket.Value] = aggregate(values);
                    if (breakOuter)
                        break;

                    values = new List<Tval>();
                    lastTimeBucket = enumerator.Current.Key;
                }

                if (item.Value.HasValue)
                    values.Add(item.Value.Value);
            }

            if (lastTimeBucket.HasValue && values.Count > 0)
                Resampled[lastTimeBucket.Value] = aggregate(values);
        }
    }
}