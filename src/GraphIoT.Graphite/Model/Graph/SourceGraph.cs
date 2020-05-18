using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class SourceGraph : GraphiteGraph
    {
        private readonly GraphViewModel _graphViewModel;

        public override string Name => _graphViewModel.Name;
        public override DateTime Begin => _graphViewModel.Begin;
        public override TimeSpan Spacing => _graphViewModel.Spacing;

        public override IEnumerable<double?> Points => _graphViewModel.Points.Select(p => p switch
        {
            bool boolVal => boolVal ? (double?)1d : 0d,
            int intVal => intVal,
            double doubleVal => doubleVal,
            null => null,
            _ => throw new NotImplementedException("type not supported yet")
        });

        public SourceGraph(GraphViewModel graphViewModel)
            => _graphViewModel = graphViewModel;
    }
}