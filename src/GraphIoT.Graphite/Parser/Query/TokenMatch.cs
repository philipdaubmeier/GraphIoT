using System.Diagnostics;
using System.Text;

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

        public string Unescaped()
        {
            if (TokenType != TokenType.StringValue)
                return Value;

            if (Value.Length < 2)
                return string.Empty;

            var val = Value[1..^1];
            var result = new StringBuilder(val.Length);
            bool escapeChar = false;
            foreach (char c in val)
            {
                if (!escapeChar && c == '\\')
                {
                    escapeChar = true;
                    continue;
                }

                if (escapeChar && c != '\\' && c != '\'')
                    throw new LexerException($"Unknown escape sequence '\\{c}'");

                result.Append(c);
                escapeChar = false;
            }

            return result.ToString();
        }
    }
}