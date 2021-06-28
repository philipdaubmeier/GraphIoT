using PhilipDaubmeier.GraphIoT.Core.ViewModel;
using PhilipDaubmeier.GraphIoT.Graphite.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public class Parser
    {
        private Queue<TokenMatch> _tokenSequence = new(0);
        private TokenMatch _lookahead = new(TokenType.SequenceTerminator);

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
                        return ReadFunction(identifier.Value);
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

        private IGraphiteExpression ReadFunction(string functionName)
        {
            if (!GraphiteFunctionMap.TryGetFunction(functionName, out GraphiteFunctionMap.FunctionDef funcDef))
                throw new ParserException($"Unknown function: '{functionName}'");

            ReadToken(TokenType.OpenParanthesis);

            var paramObjects = new List<object>();
            foreach ((var index, var param) in Enumerable.Range(0, funcDef.Params.Count).Zip(funcDef.Params))
            {
                if (index != 0)
                    ReadToken(TokenType.Comma);

                paramObjects.Add(ReadParam(param.Type));
            }

            ReadToken(TokenType.CloseParanthesis);

            var funcExpr = funcDef.Instantiate(paramObjects.ToArray());
            if (funcExpr is null)
                throw new ParserException($"Could not instantiate function '{functionName}'");

            return funcExpr;
        }

        private object ReadParam(string type)
        {
            return type switch
            {
                "seriesList" => ReadExpression(),
                "string" => ReadStringParam(),
                "int" => ReadNumberParam(),
                "boolean" => ReadBooleanParam(),
                "select" => ReadStringParam(),
                "int_or_interval" => ReadTimeParam(),
                _ => throw new ParserException($"Error in function parameter definition, unknown parameter type '{type}'")
            };
        }

        private double ReadNumberParam()
        {
            var numberRaw = _lookahead.TokenType switch
            {
                TokenType.StringValue => ReadToken(TokenType.StringValue).Unescaped(),
                _ => ReadToken(TokenType.NumberValue).Value
            };

            if (!double.TryParse(numberRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                throw new ParserException($"Expected a valid floating point number but found: '{numberRaw}'");
            return result;
        }

        private string ReadStringParam()
        {
            return ReadToken(TokenType.StringValue).Unescaped();
        }

        private bool ReadBooleanParam()
        {
            var current = _lookahead;
            return current.TokenType switch
            {
                TokenType.TrueLiteral => ReadToken(TokenType.TrueLiteral) != null,
                TokenType.FalseLiteral => ReadToken(TokenType.FalseLiteral) == null,
                _ => throw new ParserException($"Expected boolean but found: '{current.Value}' at pos {current.StartIndex}")
            };
        }

        private TimeSpan ReadTimeParam()
        {
            var str = ReadToken(TokenType.StringValue).Unescaped();
            if (!("+" + str).TryParseGraphiteOffset(out TimeSpan result))
                throw new ParserException($"Expected a valid time offset string but found: '{str}'");
            return result;
        }
    }
}