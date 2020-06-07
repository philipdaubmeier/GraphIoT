using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("offset", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("amount", "int", true)]
    public class OffsetFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly double _amount;

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x is null ? null : x + _amount, n => n));

        public OffsetFunctionExpression(IGraphiteExpression innerExpression, double amount)
            => (InnerExpression, _amount) = (innerExpression, amount);
    }
}