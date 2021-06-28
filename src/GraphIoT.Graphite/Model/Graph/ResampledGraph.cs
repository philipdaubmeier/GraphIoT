using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class ResampledGraph : GraphiteGraph
    {
        private readonly IGraphiteGraph _operand;
        private readonly Aggregator _func;
        private readonly TimeSpan _spacing;

        public override string Name => _operand.Name;
        public override DateTime Begin => _operand.Begin;
        public override TimeSpan Spacing => new(Math.Max(_operand.Spacing.Ticks, _spacing.Ticks));

        public override IEnumerable<double?> Points
        {
            get
            {
                var series = ToTimeSeries(_operand.Points, _operand.Begin, _operand.Spacing);
                if (series is null)
                    return _operand.Points;

                var newSpan = new TimeSeriesSpan(series.Begin, series.End, Spacing);
                var resampler = new TimeSeriesResampler<TimeSeries<double>, double>(newSpan, SamplingConstraint.NoOversampling);
                resampler.Aggregate(series, _func);
                return resampler.Resampled.Select(x => x.Value);
            }
        }

        public ResampledGraph(IGraphiteGraph operand, TimeSpan spacing, Aggregator func)
            => (_operand, _spacing, _func) = (operand, spacing, func);

        private static TimeSeries<double>? ToTimeSeries(IEnumerable<double?> points, DateTime begin, TimeSpan spacing)
        {
            var values = points.ToList();
            if (values.Count <= 0)
                return null;
            var series = new TimeSeries<double>(new TimeSeriesSpan(begin, begin + spacing * values.Count, values.Count));
            var i = 0;
            foreach (var value in values)
                series[i++] = value;
            return series;
        }
    }
}