using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("delay", "Transform")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("steps", "int", true)]
    public class DelayFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly int _steps;

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphTransform(g, TransformDelay));

        public DelayFunctionExpression(IGraphiteExpression innerExpression, double steps)
            => (InnerExpression, _steps) = (innerExpression, (int)steps);

        private IEnumerable<double?> TransformDelay(IEnumerable<double?> source)
        {
            foreach (var _ in Enumerable.Range(0, _steps))
                yield return null;

            foreach (var value in source.SkipLast(_steps))
                yield return value;
        }
    }
}