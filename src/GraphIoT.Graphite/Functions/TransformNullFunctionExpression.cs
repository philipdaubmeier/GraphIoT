using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("transformNull", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("default", "int", false)]
    public class TransformNullFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly double _defaultVal;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x is null ? _defaultVal : x, n => n));

        public TransformNullFunctionExpression(IGraphiteExpression innerExpression, double defaultVal)
            => (_innerExpression, _defaultVal) = (innerExpression, defaultVal);
    }
}