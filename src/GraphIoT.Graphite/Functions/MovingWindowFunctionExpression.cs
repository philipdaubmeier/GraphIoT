using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("movingWindow", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "string", true, "1min,5min,10min,30min,1hour,1day")]
    [GraphiteParam("aggregator", "string", true, "avg,avg_zero,median,sum,min,max,diff,stddev,count,range,last")]
    public class MovingWindowFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly TimeSpan _spacing;
        private readonly Aggregator _func;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new ResampledGraph(g, _spacing, _func));

        public MovingWindowFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing, Aggregator func)
            => (_innerExpression, _spacing, _func) = (innerExpression, spacing, func);
    }

    [GraphiteFunction("movingAverage", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "string", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingAverageFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingAverageFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, Aggregator.Average) { }
    }

    [GraphiteFunction("movingSum", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "string", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingSumFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingSumFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, Aggregator.Sum) { }
    }

    [GraphiteFunction("movingMax", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "string", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingMaxFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingMaxFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, Aggregator.Maximum) { }
    }

    [GraphiteFunction("movingMin", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "string", true, "1min,5min,10min,30min,1hour,1day")]
    public class MovingMinFunctionExpression : MovingWindowFunctionExpression
    {
        public MovingMinFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, Aggregator.Minimum) { }
    }
}