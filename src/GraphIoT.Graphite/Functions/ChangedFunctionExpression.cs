using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("changed", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    public class ChangedFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphTransform(g, TransformChanged));

        public ChangedFunctionExpression(IGraphiteExpression innerExpression)
            => (_innerExpression) = (innerExpression);

        private IEnumerable<double?> TransformChanged(IEnumerable<double?> source)
        {
            double? prev = null;
            foreach (var value in source)
            {
                if (prev is null)
                    prev = value;

                if (prev is null || value is null)
                {
                    yield return 0;
                    continue;
                }

                yield return value.Value != prev.Value ? 1 : 0;
            }
        }
    }
}