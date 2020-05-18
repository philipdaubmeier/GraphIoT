using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class MovingWindowFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly TimeSpan _spacing;
        private readonly Aggregator _func;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new ResampledGraph(g, _spacing, _func));

        public MovingWindowFunctionExpression(IGraphiteExpression innerExpression, TimeSpan spacing, Aggregator func)
            => (_innerExpression, _spacing, _func) = (innerExpression, spacing, func);
    }
}