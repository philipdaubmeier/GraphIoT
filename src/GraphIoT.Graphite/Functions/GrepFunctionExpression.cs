using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("grep", "Filter Series")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("grep", "string", true)]
    public class GrepFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly Regex _grep;

        public IEnumerable<IGraphiteGraph> Graphs =>
            InnerExpression.Graphs.Where(g => _grep.IsMatch(g.Name));

        public GrepFunctionExpression(IGraphiteExpression innerExpression, string grep)
            => (InnerExpression, _grep) = (innerExpression, new Regex(grep));
    }

    [GraphiteFunction("exclude", "Filter Series")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("exclude", "string", true)]
    public class ExcludeFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly Regex _exclude;

        public IEnumerable<IGraphiteGraph> Graphs =>
            InnerExpression.Graphs.Where(g => !_exclude.IsMatch(g.Name));

        public ExcludeFunctionExpression(IGraphiteExpression innerExpression, string exclude)
            => (InnerExpression, _exclude) = (innerExpression, new Regex(exclude));
    }
}