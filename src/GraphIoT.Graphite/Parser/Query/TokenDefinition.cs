using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public class TokenDefinition
    {
        private readonly Regex _regex;
        private readonly TokenType _returnsToken;
        private readonly int _precedence;

        public TokenDefinition(TokenType returnsToken, string regex, int precedence)
        {
            _regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            _returnsToken = returnsToken;
            _precedence = precedence;
        }

        public IEnumerable<TokenMatch> FindMatches(string input)
        {
            var matches = _regex.Matches(input);
            return matches.Select(m => new TokenMatch(_returnsToken, m.Value, m.Index, m.Index + m.Length - 1, _precedence));
        }
    }
}