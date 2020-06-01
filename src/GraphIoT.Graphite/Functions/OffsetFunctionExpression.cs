using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("offset", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("amount", "int", true)]
    public class OffsetFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly double _amount;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x is null ? null : x + _amount, n => n));

        public OffsetFunctionExpression(IGraphiteExpression innerExpression, double amount)
            => (_innerExpression, _amount) = (innerExpression, amount);
    }
}