using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("asPercent", "Calculate")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("total", "seriesList", true)]
    public class AsPercentFuncionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly IGraphiteGraph _total;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphBinary(g, _total, (x, t) => x.HasValue && t.HasValue ? x.Value / t.Value * 100 : (double?)null));

        public AsPercentFuncionExpression(IGraphiteExpression innerExpression, IGraphiteExpression total)
            => (_innerExpression, _total) = (innerExpression, total.Graphs.FirstOrDefault() ?? new NullGraph());
    }
}