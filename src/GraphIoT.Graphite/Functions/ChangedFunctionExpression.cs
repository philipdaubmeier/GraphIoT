using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("changed", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    public class ChangedFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphTransform(g, TransformChanged));

        public ChangedFunctionExpression(IGraphiteExpression innerExpression)
            => (InnerExpression) = (innerExpression);

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