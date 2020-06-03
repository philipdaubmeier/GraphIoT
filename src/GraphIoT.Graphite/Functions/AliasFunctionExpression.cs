using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [GraphiteFunction("alias", "Alias")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("alias", "string", true)]
    public class AliasFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly string _alias;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x, n => _alias));

        public AliasFunctionExpression(IGraphiteExpression innerExpression, string alias)
            => (_innerExpression, _alias) = (innerExpression, alias);
    }

    [GraphiteFunction("aliasSub", "Alias")]
    [GraphiteParam("seriesList", "seriesList", true)]
    [GraphiteParam("search", "string", true)]
    [GraphiteParam("replace", "string", true)]
    public class AliasSubFunctionExpression : IGraphiteExpression
    {
        private readonly IGraphiteExpression _innerExpression;
        private readonly Regex _search;
        private readonly string _replace;

        public IEnumerable<IGraphiteGraph> Graphs => _innerExpression.Graphs
            .Select(g => new DerivedGraphUnary(g, x => x, n => _search.Replace(n, _replace)));

        public AliasSubFunctionExpression(IGraphiteExpression innerExpression, string search, string replace)
            => (_innerExpression, _search, _replace) = (innerExpression, new Regex(search), replace);
    }
}