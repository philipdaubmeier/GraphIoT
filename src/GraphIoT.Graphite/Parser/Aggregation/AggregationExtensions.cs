using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using System;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser
{
    public static class AggregationExtensions
    {
        public static Aggregator ToAggregator(this string aggregator)
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
}