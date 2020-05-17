using System.Diagnostics;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    [DebuggerDisplay("[{StartIndex}..{EndIndex}]; {TokenType,nq}; {Value.ToString()}")]
    public class TokenMatch
    {
        public TokenType TokenType { get; }
        public string Value { get; } = string.Empty;
        public int StartIndex { get; } = 0;
        public int EndIndex { get; } = 0;
        public int Precedence { get; } = 0;

        public TokenMatch(TokenType type) => TokenType = type;

        public TokenMatch(TokenType type, string value, int start, int end, int precedence)
            => (TokenType, Value, StartIndex, EndIndex, Precedence) = (type, value, start, end, precedence);
    }
}