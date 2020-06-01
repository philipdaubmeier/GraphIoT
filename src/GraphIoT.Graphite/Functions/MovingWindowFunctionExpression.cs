using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("movingWindow", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    [GraphiteParam("aggregator", "string", true, "avg,avg_zero,median,sum,min,max,diff,stddev,count,range,last")]
    public class MovingWindowFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly TimeSpan _spacing;
        private readonly Aggregator _func;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new ResampledGraph(g, _spacing, _func));

        public MovingWindowFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing, string func)
            => (_innerExpression, _spacing, _func) = (innerExpression, spacing, ToAggregator(func));

        private static Aggregator ToAggregator(string aggregator)
        {
            return aggregator switch
            {
                var x when
                    x == "avg" ||
                    x == "average" => Aggregator.Average,
                "avg_zero" => Aggregator.AverageZero,
                "median" => Aggregator.Median,
                var x when
                    x == "sum" ||
                    x == "total" => Aggregator.Sum,
                "min" => Aggregator.Minimum,
                "max" => Aggregator.Maximum,
                "diff" => Aggregator.Diff,
                "stddev" => Aggregator.Stddev,
                "count" => Aggregator.Count,
                var x when
                    x == "range" ||
                    x == "rangeOf" => Aggregator.Range,
                "multiply" => Aggregator.Multiply,
                var x when
                    x == "last" ||
                    x == "current" => Aggregator.Last,
                _ => throw new Exception($"Unrecognized aggregator function '{aggregator}'")
            };
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