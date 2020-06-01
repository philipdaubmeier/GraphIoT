using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("scale", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("factor", "int", true)]
    public class ScaleFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly double _factor;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x is null ? null : x * _factor, n => n));

        public ScaleFunctionExpression(IGraphiteExpression innerExpression, double factor)
            => (_innerExpression, _factor) = (innerExpression, factor);
    }
}