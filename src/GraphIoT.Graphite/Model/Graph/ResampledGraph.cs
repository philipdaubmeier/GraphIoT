using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class ResampledGraph : GraphiteGraph
    {
        private readonly IGraphiteGraph _operand;
        private readonly Aggregator _func;

        public override string Name => _operand.Name;
        public override DateTime Begin => _operand.Begin;
        public override TimeSpan Spacing { get; }

        public override IEnumerable<double?> Points
        {
            get
            {
                var series = ToTimeSeries(Points, Begin, Spacing);
                var newSpan = new TimeSeriesSpan(series.Begin, series.End, Spacing);
                var resampler = new TimeSeriesResampler<TimeSeries<double>, double>(newSpan, SamplingConstraint.NoOversampling);
                Func<IEnumerable<double>, double> aggregate = _func switch
                {
                    Aggregator.Minimum => x => x.Min(),
                    Aggregator.Maximum => x => x.Max(),
                    Aggregator.Sum => x => x.Sum(),
                    _ => x => x.Average()
                };

                resampler.SampleAggregate(series, aggregate);
                return resampler.Resampled.Select(x => x.Value);
            }
        }

        public ResampledGraph(IGraphiteGraph operand, TimeSpan spacing, Aggregator func)
            => (_operand, Spacing, _func) = (operand, spacing, func);

        private static TimeSeries<double> ToTimeSeries(IEnumerable<double?> points, DateTime begin, TimeSpan spacing)
        {
            var values = points.ToList();
            var series = new TimeSeries<double>(new TimeSeriesSpan(begin, begin + spacing * values.Count, values.Count));
            var i = 0;
            foreach (var value in values)
                series[i++] = value;
            return series;
        }
    }
}