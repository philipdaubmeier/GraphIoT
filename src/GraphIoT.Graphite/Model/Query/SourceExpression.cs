using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class SourceExpression : IGraphiteExpression
    {
        public IEnumerable<IGraphiteGraph> Graphs { get; }

        public SourceExpression(List<IGraphiteGraph> graphs) => Graphs = graphs;

        public static SourceExpression CreateFromQueryExpression(GraphDataSource dataSource, IEnumerable<string> queryExpression)
        {
            return new SourceExpression(dataSource.Query(queryExpression).Select(g => new SourceGraph(g)).Cast<IGraphiteGraph>().ToList());
        }
    }
}