using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("countSeries", "Combine")]
    [GraphiteParam("seriesList", "seriesList", true)]
    public class CountSeriesFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly int _graphCount;

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => _graphCount, n => n));

        public CountSeriesFunctionExpression(IGraphiteExpression innerExpression)
            => (InnerExpression, _graphCount) = (innerExpression, innerExpression.Graphs.Count());
    }
}