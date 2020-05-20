using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class DerivedGraphUnary : GraphiteGraph
    {
        private readonly IGraphiteGraph _operand;
        private readonly Func<double?, double?> _operation;
        private readonly Func<string, string> _nameTransform;

        public override string Name => _nameTransform(_operand.Name);
        public override DateTime Begin => _operand.Begin;
        public override TimeSpan Spacing => _operand.Spacing;

        public override IEnumerable<double?> Points => _operand.Points.Select(_operation);

        public DerivedGraphUnary(IGraphiteGraph operand, Func<double?, double?> operation, Func<string, string> nameTransform)
            => (_operand, _operation, _nameTransform) = (operand, operation, nameTransform);
    }
}