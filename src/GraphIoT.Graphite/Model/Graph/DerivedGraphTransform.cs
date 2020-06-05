using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class DerivedGraphTransform : GraphiteGraph
    {
        private readonly IGraphiteGraph _operand;
        private readonly Func<IEnumerable<double?>, IEnumerable<double?>> _transform;

        public override string Name => _operand.Name;
        public override DateTime Begin => _operand.Begin;
        public override TimeSpan Spacing => _operand.Spacing;

        public override IEnumerable<double?> Points => _transform(_operand.Points);

        public DerivedGraphTransform(IGraphiteGraph operand, Func<IEnumerable<double?>, IEnumerable<double?>> transform)
            => (_operand, _transform) = (operand, transform);
    }
}