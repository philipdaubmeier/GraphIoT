using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    /// <summary>
    /// Graphite query lexer. This is an improved version of the precedence based regex tokenizer found here:
    /// https://jack-vanlightly.com/blog/2016/2/24/a-more-efficient-regex-tokenizer
    /// </summary>
    public class Lexer
    {
        private static readonly List<TokenDefinition> _tokenDefinitions = new List<TokenDefinition>()
        {
            new TokenDefinition(TokenType.Identifier, @"[\w\.\*:_]+( [\w\.\*:_]+)*", 3),
            new TokenDefinition(TokenType.Comma, @",", 1),
            new TokenDefinition(TokenType.OpenParanthesis, @"\(", 1),
            new TokenDefinition(TokenType.CloseParanthesis, @"\)", 1),
            new TokenDefinition(TokenType.StringValue, @"'[^']*'", 2),
            new TokenDefinition(TokenType.NumberValue, @"\d+(\.\d+)?", 1)
        };

        public IEnumerable<TokenMatch> Tokenize(string input)
        {
            var tokenMatches = _tokenDefinitions
                .SelectMany(def => def.FindMatches(input).ToList())
                .GroupBy(match => match.StartIndex)
                .OrderBy(match => match.Key)
                .ToList();

            TokenMatch? lastMatch = null;
            foreach (var match in tokenMatches)
            {
                var bestMatch = match.OrderBy(m => m.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex <= lastMatch.EndIndex)
                    continue;

                if (lastMatch is null && bestMatch.StartIndex > 0)
                    CheckWhitespace(input, 0, bestMatch.StartIndex);

                if (lastMatch != null && lastMatch.EndIndex + 1 < bestMatch.StartIndex)
                    CheckWhitespace(input, lastMatch.EndIndex + 1, bestMatch.StartIndex - lastMatch.EndIndex - 1);

                yield return bestMatch;
                lastMatch = bestMatch;
            }

            yield return new TokenMatch(TokenType.SequenceTerminator);
        }

        private void CheckWhitespace(string input, int start, int length)
        {
            var slice = input.Substring(start, Math.Min(input.Length - start, length));

            if (!string.IsNullOrWhiteSpace(slice))
                throw new LexerException($"Unrecognized sequence at pos {start}: '{slice.Substring(0, Math.Min(slice.Length, 10))}'");
        }
    }
}