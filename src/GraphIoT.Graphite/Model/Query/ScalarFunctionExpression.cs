using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class ScalarFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly Func<double?, double?> _operation;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, _operation));

        public ScalarFunctionExpression(IGraphiteExpression innerExpression, Func<double?, double?> operation)
            => (_innerExpression, _operation) = (innerExpression, operation);
    }
}