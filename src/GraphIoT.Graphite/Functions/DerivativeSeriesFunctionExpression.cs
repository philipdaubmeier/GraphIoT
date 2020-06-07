using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("derivative", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    public class DerivativeSeriesFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphTransform(g, TransformDerivative));

        public DerivativeSeriesFunctionExpression(IGraphiteExpression innerExpression)
            => (InnerExpression) = (innerExpression);

        private IEnumerable<double?> TransformDerivative(IEnumerable<double?> source)
        {
            double? prev = null;
            foreach (var value in source)
            {
                if (value.HasValue && prev.HasValue)
                    yield return value.Value - prev.Value;
                else
                    yield return null;

                prev = value;
            }
        }
    }
}