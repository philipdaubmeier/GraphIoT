using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class AliasFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly string _alias;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x, n => _alias));

        public AliasFunctionExpression(IGraphiteExpression innerExpression, string alias)
            => (_innerExpression, _alias) = (innerExpression, alias);
    }
}