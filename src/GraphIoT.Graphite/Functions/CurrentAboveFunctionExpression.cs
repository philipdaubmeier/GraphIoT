using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("currentAbove", "Filter Series")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("n", "int", true)]
    public class CurrentAboveFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly double _n;

        public IEnumerable<IGraphiteGraph> Graphs =>
            InnerExpression.Graphs.Where(g => (g.Points.LastOrDefault() ?? double.MinValue) >= _n);

        public CurrentAboveFunctionExpression(IGraphiteExpression innerExpression, double n)
            => (InnerExpression, _n) = (innerExpression, n);
    }

    [GraphiteFunction("currentBelow", "Filter Series")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("n", "int", true)]
    public class CurrentBelowFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly double _n;

        public IEnumerable<IGraphiteGraph> Graphs =>
            InnerExpression.Graphs.Where(g => (g.Points.LastOrDefault() ?? double.MinValue) <= _n);

        public CurrentBelowFunctionExpression(IGraphiteExpression innerExpression, double n)
            => (InnerExpression, _n) = (innerExpression, n);
    }
}