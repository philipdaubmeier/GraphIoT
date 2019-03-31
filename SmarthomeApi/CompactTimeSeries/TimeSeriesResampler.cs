using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactTimeSeries
{
    public enum SamplingConstraint
    {
        SampleAny,
        NoOversampling
    }

    public class TimeSeriesResampler<Tseries, Tval> where Tval : struct where Tseries : TimeSeriesBase<Tval>
    {
        private ITimeSeries<Tval> resampled = null;
        public ITimeSeries<Tval> Resampled
        {
            get
            {
                CreateDeferred(Span);
                return resampled;
            }
            private set
            {
                resampled = value;
            }
        }

        public TimeSeriesSpan Span { get; private set; }

        public SamplingConstraint Constraint { get; private set; }

        public TimeSeriesResampler(TimeSeriesSpan span) : this(span, SamplingConstraint.SampleAny) { }

        public TimeSeriesResampler(TimeSeriesSpan span, SamplingConstraint constraint)
        {
            Span = span;
            Constraint = constraint;
        }

        private void CreateDeferred(TimeSeriesSpan oldSpan)
        {
            if (resampled != null)
                return;

            if (Constraint == SamplingConstraint.NoOversampling && Span.Duration < oldSpan.Duration)
                Span = new TimeSeriesSpan(Span.Begin, Span.End, oldSpan.Duration);

            resampled = (ITimeSeries<Tval>)Activator.CreateInstance(typeof(Tseries), Span);
        }
        
        public void SampleAccumulate(IEnumerable<ITimeSeries<Tval>> timeseries)
        {
            foreach (var series in timeseries)
                SampleAccumulate(series);
        }

        public void SampleAverage(IEnumerable<ITimeSeries<Tval>> timeseries, Func<Tval, decimal> selector, Func<decimal, Tval> resultCast)
        {
            foreach (var series in timeseries)
                SampleAverage(series, selector, resultCast);
        }

        public void SampleAggregate(IEnumerable<ITimeSeries<Tval>> timeseries, Func<IEnumerable<Tval>, Tval> aggregate)
        {
            foreach (var series in timeseries)
                SampleAggregate(series, aggregate);
        }

        public void SampleAccumulate(ITimeSeries<Tval> timeseries)
        {
            CreateDeferred(timeseries.Span);
            foreach (var item in timeseries)
                if (item.Value.HasValue)
                    Resampled.Accumulate(item.Key, item.Value.Value);
        }

        public void SampleAverage(ITimeSeries<Tval> timeseries, Func<Tval, decimal> selector, Func<decimal, Tval> resultCast)
        {
            SampleAggregate(timeseries, x => resultCast(x.Average(selector)));
        }

        public void SampleAggregate(ITimeSeries<Tval> timeseries, Func<IEnumerable<Tval>, Tval> aggregate)
        {
            CreateDeferred(timeseries.Span);

            DateTime? lastTimeBucket = null;
            List<Tval> values = new List<Tval>();
            bool breakOuter = false;
            int i = 0;

            foreach (var item in timeseries)
            {
                if (item.Key < Resampled.Span.Begin)
                    continue;

                var timebucket = Resampled.Span.Begin + i * Resampled.Span.Duration;
                while (timebucket > item.Key || (timebucket + Resampled.Span.Duration) <= item.Key)
                {
                    i++;
                    if (i >= Resampled.Span.Count)
                    {
                        breakOuter = true;
                        break;
                    }
                    else
                        timebucket = Resampled.Span.Begin + i * Resampled.Span.Duration;
                }

                if (!lastTimeBucket.HasValue)
                    lastTimeBucket = timebucket;

                if (breakOuter || lastTimeBucket.Value != timebucket)
                {
                    if (values.Count > 0)
                        Resampled[lastTimeBucket.Value] = aggregate(values);
                    if (breakOuter)
                        break;

                    values = new List<Tval>();
                    lastTimeBucket = timebucket;
                }

                if (item.Value.HasValue)
                    values.Add(item.Value.Value);
            }

            if (lastTimeBucket.HasValue && values.Count > 0)
                Resampled[lastTimeBucket.Value] = aggregate(values);
        }
    }
}