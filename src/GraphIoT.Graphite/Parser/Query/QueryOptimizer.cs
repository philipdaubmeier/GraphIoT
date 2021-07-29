using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Graphite.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public class QueryOptimizer
    {
        private readonly HashSet<ResampleFunctionExpression?> _resampleFuncs = new();

        private readonly List<Aggregator> _nonOptimizableResampler = new()
        {
            Aggregator.Median,
            Aggregator.Minimum,
            Aggregator.Maximum,
            Aggregator.Diff,
            Aggregator.Stddev,
            Aggregator.Count,
            Aggregator.Range,
            Aggregator.Multiply,
            Aggregator.Last
        };

        public readonly Parser _parser;
        public GraphDataSource DataSource { get; set; } = new GraphDataSource(new List<IGraphCollectionViewModel>());

        public QueryOptimizer(Parser parser) => (_parser, DataSource) = (parser, parser.DataSource);

        public IEnumerable<IGraphiteGraph> ParseAndOptimize(IEnumerable<string> target)
        {
            var expressions = target.Select(t => _parser.Parse(t));
            Optimize(expressions);

            return expressions.SelectMany(x => x.Graphs);
        }

        public void Optimize(IEnumerable<IGraphiteExpression> expressions)
        {
            foreach (var expression in expressions)
                TraverseAst(expression, Enumerable.Empty<IGraphiteExpression>());

            // if AST contains one or more queries without any additional
            // resampling steps there is nothing to optimize
            if (_resampleFuncs.Contains(null))
                return;

            // no optimization potential if desired resolution is not lower than the original
            var originalResolution = DataSource.Span;
            var minReamplingRate = _resampleFuncs.Where(x => x != null).Min(x => x!.Spacing);
            if (minReamplingRate <= originalResolution.Duration)
                return;

            // no optimization potential if all the resampling functions contain non-optimizable aggregator functions
            if (_resampleFuncs.All(x => x is null || _nonOptimizableResampler.Contains(x.Func)))
                return;

            // optimize by loading lower resolution source data
            DataSource.Span = new TimeSeriesSpan(originalResolution.Begin, originalResolution.End, minReamplingRate);
        }

        private bool TraverseAst(IGraphiteExpression expression, IEnumerable<IGraphiteExpression> parents)
        {
            return expression switch
            {
                IFunctionExpression funcExpr => TraverseAst(funcExpr.InnerExpression, parents.Prepend(expression)),
                SourceExpression _ => AddSource(parents),
                _ => throw new Exception("Unknown expression type found in AST.")
            };
        }

        private bool AddSource(IEnumerable<IGraphiteExpression> parents)
        {
            _resampleFuncs.Add(parents.Select(x => x as ResampleFunctionExpression).FirstOrDefault());
            return true;
        }
    }
}