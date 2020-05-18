using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Graphite.Model;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public class Parser
    {
        private Queue<TokenMatch> _tokenSequence = new Queue<TokenMatch>(0);
        private TokenMatch _lookahead = new TokenMatch(TokenType.SequenceTerminator);

        public GraphDataSource DataSource { get; set; } = new GraphDataSource(new List<IGraphCollectionViewModel>());

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
                throw new ParserException($"Expected {tokenType.ToString().ToUpper()} but found: '{current.Value}' at pos {current.StartIndex}");

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
                        var innerExpr = ReadExpression();
                        var expr = identifier.Value switch
                        {
                            "movingAverage" => ReadMovingWindowFunction(innerExpr, Aggregator.Average),
                            "movingSum" => ReadMovingWindowFunction(innerExpr, Aggregator.Sum),
                            "movingMax" => ReadMovingWindowFunction(innerExpr, Aggregator.Maximum),
                            "movingMin" => ReadMovingWindowFunction(innerExpr, Aggregator.Minimum),
                            "movingWindow" => ReadMovingWindowFunction(innerExpr),
                            "offset" => ReadOffsetFunction(innerExpr),
                            _ => throw new ParserException($"Unrecognized function '{identifier}'")
                        };
                        ReadToken(TokenType.CloseParanthesis);
                        return expr;
                    }
                case TokenType.Comma:
                case TokenType.CloseParanthesis:
                case TokenType.SequenceTerminator:
                    {
                        return SourceExpression.CreateFromQueryExpression(DataSource, queryExpression: identifier.Value);
                    }
                default:
                    throw new ParserException($"Unexpected {_lookahead.TokenType.ToString().ToUpper()}");
            }
        }

        private IGraphiteExpression ReadOffsetFunction(IGraphiteExpression seriesParam)
        {
            var offset = ReadDoubleParam();
            return new ScalarFunctionExpression(seriesParam, value => value + offset);
        }

        private IGraphiteExpression ReadMovingWindowFunction(IGraphiteExpression seriesParam, Aggregator? func = null)
        {
            var windowSize = ReadTimeParam();
            var aggregate = func ?? Aggregator.Default;
            if (aggregate == Aggregator.Default)
            {
                var aggregatorParam = ReadStringParam();
                aggregate = aggregatorParam switch
                {
                    "min" => Aggregator.Minimum,
                    "max" => Aggregator.Maximum,
                    "sum" => Aggregator.Sum,
                    var x when x == "avg" ||
                               x == "average" => Aggregator.Average,
                    _ => throw new ParserException($"Unrecognized aggregator function '{aggregatorParam}'")
                };
            }
            return new MovingWindowFunctionExpression(seriesParam, windowSize, aggregate);
        }

        private double ReadDoubleParam()
        {
            ReadToken(TokenType.Comma);
            var numberToken = ReadToken(TokenType.NumberValue);
            if (!double.TryParse(numberToken.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                throw new ParserException($"Expected a valid floating point number but found: '{numberToken.Value}' at pos {numberToken.StartIndex}");
            return result;
        }

        private string ReadStringParam()
        {
            ReadToken(TokenType.Comma);
            var stringToken = ReadToken(TokenType.StringValue);
            var rawString = stringToken.Value;
            if (rawString.Length < 2 || rawString[0] != '\'' || rawString[^1] != '\'')
                throw new ParserException($"Expected a valid string but found: '{stringToken.Value}' at pos {stringToken.StartIndex}");
            return rawString[1..^1];
        }

        private TimeSpan ReadTimeParam()
        {
            var str = ReadStringParam();
            if (!("+" + str).TryParseGraphiteOffset(out TimeSpan result))
                throw new ParserException($"Expected a valid time offset string but found: '{str}'");
            return result;
        }
    }
}