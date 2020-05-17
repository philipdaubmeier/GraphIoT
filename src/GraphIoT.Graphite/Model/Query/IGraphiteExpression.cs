using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public interface IGraphiteExpression
    {
        IEnumerable<IGraphiteGraph> Graphs { get; }
    }
}