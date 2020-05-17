using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class SourceExpression : IGraphiteExpression
    {
        public IEnumerable<IGraphiteGraph> Graphs { get; }

        public SourceExpression(List<IGraphiteGraph> graphs) => Graphs = graphs;
    }
}