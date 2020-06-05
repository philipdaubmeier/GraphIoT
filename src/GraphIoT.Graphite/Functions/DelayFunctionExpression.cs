using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("delay", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("steps", "int", true)]
    public class DelayFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly int _steps;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphTransform(g, TransformDelay));

        public DelayFunctionExpression(IGraphiteExpression innerExpression, double steps)
            => (_innerExpression, _steps) = (innerExpression, (int)steps);

        private IEnumerable<double?> TransformDelay(IEnumerable<double?> source)
        {
            foreach (var _ in Enumerable.Range(0, _steps))
                yield return null;

            foreach (var value in source.SkipLast(_steps))
                yield return value;
        }
    }
}