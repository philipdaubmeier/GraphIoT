using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.CompactTimeSeries
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
                CreateDeferred(Span.Duration);
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

        private void CreateDeferred(TimeSpan oldInterval)
        {
            if (resampled != null)
                return;

            if (Constraint == SamplingConstraint.NoOversampling && Span.Duration < oldInterval)
                Span = new TimeSeriesSpan(Span.Begin, Span.End, oldInterval);

            resampled = (ITimeSeries<Tval>)Activator.CreateInstance(typeof(Tseries), Span);
        }
        
        public void SampleAccumulate(ITimeSeries<Tval> timeseries)
        {
            CreateDeferred(timeseries.Span.Duration);
            foreach (var item in timeseries)
                if (item.Value.HasValue)
                    Resampled.Accumulate(item.Key, item.Value.Value);
        }

        public void SampleAggregate(ITimeSeries<Tval> timeseries, Func<IEnumerable<Tval>, Tval> aggregate)
        {
            SampleAggregate(new List<ITimeSeries<Tval>>() { timeseries }, aggregate);
        }

        public void SampleAggregate(IEnumerable<ITimeSeries<Tval>> timeseries, Func<IEnumerable<Tval>, Tval> aggregate)
        {
            var listTimeseries = new List<ITimeSeries<Tval>>(timeseries);
            if (listTimeseries.Count <= 0)
                return;

            CreateDeferred(listTimeseries.Min(x => x.Span.Duration));

            for (int i = 0; i < Resampled.Span.Count; i++)
            {
                var timebucket = Resampled.Span.Begin + i * Resampled.Span.Duration;

                IEnumerable<Tval> ItemsToAggregate(ITimeSeries<Tval> serie)
                {
                    foreach (var item in serie)
                    {
                        if (item.Key < timebucket)
                            continue;

                        if (item.Key >= (timebucket + Resampled.Span.Duration))
                            yield break;

                        if (!item.Value.HasValue)
                            continue;

                        yield return item.Value.Value;
                    }
                }

                var values = listTimeseries
                    .Where(serie => serie.Span.End >= timebucket && serie.Span.Begin <= timebucket + Resampled.Span.Duration)
                    .SelectMany(serie => ItemsToAggregate(serie));

                Resampled[i] = values.Any() ? aggregate(values) : (Tval?)null;
            }
        }
    }
}