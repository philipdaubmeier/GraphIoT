using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class DerivedGraphBinary : GraphiteGraph
    {
        private readonly IGraphiteGraph _leftOperand;
        private readonly IGraphiteGraph _rightOperand;
        private readonly Func<double?, double?, double?> _operation;

        public override string Name => _leftOperand.Name;
        public override DateTime Begin => _leftOperand.Begin;
        public override TimeSpan Spacing => _leftOperand.Spacing;

        public override IEnumerable<double?> Points => _leftOperand.Points
            .Zip(_rightOperand.Points).Select(x => _operation(x.First, x.Second));

        public DerivedGraphBinary(IGraphiteGraph leftOperand, IGraphiteGraph rightOperand, Func<double?, double?, double?> operation)
            => (_leftOperand, _rightOperand, _operation) = (leftOperand, rightOperand, operation);
    }
}