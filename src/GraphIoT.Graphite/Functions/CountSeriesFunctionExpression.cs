using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("countSeries", "Combine")]
    [GraphiteParam("seriesList", "seriesList", true)]
    public class CountSeriesFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly int _graphCount;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => _graphCount, n => n));

        public CountSeriesFunctionExpression(IGraphiteExpression innerExpression)
            => (_innerExpression, _graphCount) = (innerExpression, innerExpression.Graphs.Count());
    }
}