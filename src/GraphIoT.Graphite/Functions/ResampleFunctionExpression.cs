using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using PhilipDaubmeier.GraphIoT.Graphite.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("resample", "Resample")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    [GraphiteParam("aggregator", "string", true, "avg,avg_zero,median,sum,min,max,diff,stddev,count,range,last")]
    public class ResampleFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly TimeSpan _spacing;
        private readonly Aggregator _func;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new ResampledGraph(g, _spacing, _func));

        public ResampleFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing, string func)
            => (_innerExpression, _spacing, _func) = (innerExpression, spacing, func.ToAggregator());
    }

    [GraphiteFunction("resampleAverage", "Resample")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class ResampleAverageFunctionExpression : ResampleFunctionExpression
    {
        public ResampleAverageFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "avg") { }
    }

    [GraphiteFunction("resampleSum", "Resample")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class ResampleSumFunctionExpression : ResampleFunctionExpression
    {
        public ResampleSumFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "sum") { }
    }

    [GraphiteFunction("resampleMax", "Resample")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class ResampleMaxFunctionExpression : ResampleFunctionExpression
    {
        public ResampleMaxFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "max") { }
    }

    [GraphiteFunction("resampleMin", "Resample")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("time", "int_or_interval", true, "1min,5min,10min,30min,1hour,1day")]
    public class ResampleMinFunctionExpression : ResampleFunctionExpression
    {
        public ResampleMinFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing)
            : base(innerExpression, spacing, "min") { }
    }
}