using PhilipDaubmeier.GraphIoT.Graphite.Model;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public class Parser
    {
        private Queue<TokenMatch> _tokenSequence = new Queue<TokenMatch>(0);
        private TokenMatch _lookahead = new TokenMatch(TokenType.SequenceTerminator);

        public Parser() { }

        public IGraphiteExpression Parse(string input)
        {
            var tokens = new Lexer().Tokenize(input);
            _tokenSequence = new Queue<TokenMatch>(tokens);

            // initialize the lookahead field with dequeueing the first token
            DiscardToken();

            var rootExpression = ReadExpression();
            ReadToken(TokenType.SequenceTerminator);
            return rootExpression;
        }

        private void DiscardToken()
        {
            if (_tokenSequence.TryDequeue(out TokenMatch? token) && token != null)
                _lookahead = token;
            else
                _lookahead = new TokenMatch(TokenType.SequenceTerminator);
        }

        private TokenMatch ReadToken(TokenType tokenType)
        {
            var current = _lookahead;
            if (current.TokenType != tokenType)
                throw new ParserException($"Expected {tokenType.ToString().ToUpper()} but found: '{current.Value}'");

            DiscardToken();
            return current;
        }

        private IGraphiteExpression ReadExpression()
        {
            var identifier = ReadToken(TokenType.Identifier);

            switch (_lookahead.TokenType)
            {
                case TokenType.OpenParanthesis:
                    {
                        ReadToken(TokenType.OpenParanthesis);
                        var expr = ReadExpression();
                        ReadToken(TokenType.CloseParanthesis);
                        return new ScalarFunctionExpression(expr, value => value * 10d);
                    }
                case TokenType.Comma:
                case TokenType.CloseParanthesis:
                case TokenType.SequenceTerminator:
                    {
                        return new SourceExpression(new List<IGraphiteGraph>());
                    }
                default:
                    throw new ParserException($"Unexpected {_lookahead.TokenType.ToString().ToUpper()}");
            }
        }
    }
}