using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("isNull", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    public class IsNullFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x is null ? 1 : 0, n => n));

        public IsNullFunctionExpression(IGraphiteExpression innerExpression)
            => InnerExpression = innerExpression;
    }
}