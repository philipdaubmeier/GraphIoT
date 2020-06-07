using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("alias", "Alias")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("alias", "string", true)]
    public class AliasFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly string _alias;

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x, n => _alias));

        public AliasFunctionExpression(IGraphiteExpression innerExpression, string alias)
            => (InnerExpression, _alias) = (innerExpression, alias);
    }

    [GraphiteFunction("aliasSub", "Alias")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("search", "string", true)]
    [GraphiteParam("replace", "string", true)]
    public class AliasSubFunctionExpression : IFunctionExpression
    {
        public IGraphiteExpression InnerExpression { get; }
        private readonly Regex _search;
        private readonly string _replace;

        public IEnumerable<IGraphiteGraph> Graphs => InnerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x, n => _search.Replace(n, _replace)));

        public AliasSubFunctionExpression(IGraphiteExpression innerExpression, string search, string replace)
            => (InnerExpression, _search, _replace) = (innerExpression, new Regex(search), replace);
    }
}