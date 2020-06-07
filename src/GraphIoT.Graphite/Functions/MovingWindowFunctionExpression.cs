using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using PhilipDaubmeier.GraphIoT.Graphite.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("movingWindow", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    [GraphiteParam("aggregator", "string", true, "avg,avg_zero,median,sum,min,max,diff,stddev,count,range,last")]
    public class MovingWindowFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly TimeSpan _spacing;
        private readonly Aggregator _func;

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphTransform(g, TransformMovingWindow));

        public MovingWindowFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing, string func)
            => (InnerExpression, _spacing, _func) = (innerExpression, spacing, func.ToAggregator());

        private IEnumerable<double?> TransformMovingWindow(IEnumerable<double?> source)
        {
            int steps = (int)Math.Max(1d, Math.Round(_spacing / (InnerExpression.Graphs.FirstOrDefault() ?? new NullGraph()).Spacing, 0));

            foreach (var _ in Enumerable.Range(0, steps))
                yield return null;

            Queue<double?> input = new Queue<double?>(source.Take(steps - 1).Append(0d));

            foreach (var value in source.Skip(steps))
            {
                input.Dequeue();
                input.Enqueue(value);

                var elements = input.Where(x => x != null).Select(x => x.Value);
                if (elements.Any())
                    yield return elements.Aggregate(_func);
                else
                    yield return null;
            }
        }
    }

    [GraphiteFunction("movingAverage", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingAverageFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingAverageFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "avg") { }
    }

    [GraphiteFunction("movingSum", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingSumFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingSumFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "sum") { }
    }

    [GraphiteFunction("movingMax", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingMaxFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingMaxFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "max") { }
    }

    [GraphiteFunction("movingMin", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingMinFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingMinFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "min") { }
    }
}