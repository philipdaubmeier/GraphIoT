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
        private Tseries? resampled = null;
        public Tseries Resampled
        {
            get
            {
                CreateDeferred(Span.Duration);
                return resampled!;
            }
            set
            {
                if (value is null)
                    return;

                resampled = value;
                Span = resampled.Span;
            }
        }

        public TimeSeriesSpan Span { get; private set; }

        public SamplingConstraint Constraint { get; private set; }

        public TimeSeriesResampler(TimeSeriesSpan span) : this(span, SamplingConstraint.SampleAny) { }

        public TimeSeriesResampler(TimeSeriesSpan span, SamplingConstraint constraint)
            => (Span, Constraint) = (span, constraint);

        private void CreateDeferred(TimeSpan oldInterval)
        {
            if (resampled != null)
                return;

            if (Constraint == SamplingConstraint.NoOversampling && Span.Duration < oldInterval)
                Span = new TimeSeriesSpan(Span.Begin, Span.End, oldInterval);

            resampled = (Tseries?)Activator.CreateInstance(typeof(Tseries), Span);
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
                var values = ItemsToAggregate(listTimeseries, Resampled.Span.Begin + i * Resampled.Span.Duration);

                if (values.Any())
                    Resampled[i] = aggregate(values);
            }
        }

        private IEnumerable<Tval> ItemsToAggregate(List<ITimeSeries<Tval>> listTimeseries, DateTime timebucket)
        {
            foreach (var serie in listTimeseries)
            {
                if (serie is null)
                    continue;

                if (!(serie.Span.End >= timebucket && serie.Span.Begin <= timebucket + Resampled.Span.Duration))
                    continue;

                var startIndex = Math.Max(0, (int)Math.Ceiling((timebucket - serie.Span.Begin) / serie.Span.Duration));
                var endIndex = Math.Min(serie.Count, (int)Math.Ceiling((timebucket + Resampled.Span.Duration - serie.Span.Begin) / serie.Span.Duration));
                for (int i = startIndex; i < endIndex; i++)
                    if (serie[i].HasValue)
                        yield return serie[i]!.Value;
            }
        }
    }
}