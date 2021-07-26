using PhilipDaubmeier.GraphIoT.Graphite.Parser.Query;
using System.Linq;
using Xunit;

namespace GraphIoT.Graphite.Tests
{
    public class QueryLexerTest
    {
        [Theory]
        [InlineData("movingWindow(root.subpath.subsubpath.leaf1,'1d27min','avg')")]
        [InlineData("movingWindow(root.subpath.subsubpath.leaf1, '1d27min', 'avg')")]
        [InlineData("      movingWindow(root.subpath.subsubpath.leaf1,'1d27min','avg')    ")]
        [InlineData("  \t \r\n  movingWindow \n\n  ( \n  root.subpath.subsubpath.leaf1    , \r\n '1d27min'   ,  \r\n  'avg'  )  \t \r\n  ")]
        public void LexerWhitepaceInvarianceTest(string query)
        {
            var lexer = new Lexer();
            var tokens = Lexer.Tokenize(query).ToList();

            Assert.Equal("movingWindow", tokens[0].Value);
            Assert.Equal("(", tokens[1].Value);
            Assert.Equal("root.subpath.subsubpath.leaf1", tokens[2].Value);
            Assert.Equal(",", tokens[3].Value);
            Assert.Equal("'1d27min'", tokens[4].Value);
            Assert.Equal(",", tokens[5].Value);
            Assert.Equal("'avg'", tokens[6].Value);
            Assert.Equal(")", tokens[7].Value);
            Assert.Equal("", tokens[8].Value);

            Assert.Equal(TokenType.Identifier, tokens[0].TokenType);
            Assert.Equal(TokenType.OpenParanthesis, tokens[1].TokenType);
            Assert.Equal(TokenType.Identifier, tokens[2].TokenType);
            Assert.Equal(TokenType.Comma, tokens[3].TokenType);
            Assert.Equal(TokenType.StringValue, tokens[4].TokenType);
            Assert.Equal(TokenType.Comma, tokens[5].TokenType);
            Assert.Equal(TokenType.StringValue, tokens[6].TokenType);
            Assert.Equal(TokenType.CloseParanthesis, tokens[7].TokenType);
            Assert.Equal(TokenType.SequenceTerminator, tokens[8].TokenType);
        }

        [Theory]
        [InlineData("(root.subpath.subsubpath.leaf1)", "root.subpath.subsubpath.leaf1")]
        [InlineData("(ROot.SubpATh.SubSubPath.lEAF1)", "ROot.SubpATh.SubSubPath.lEAF1")]
        [InlineData("(ROOT.SUBPATH.SUBSUBPATH.LEAF1)", "ROOT.SUBPATH.SUBSUBPATH.LEAF1")]
        public void LexerCaseSensitivityTest(string query, string expected)
        {
            var lexer = new Lexer();
            var tokens = Lexer.Tokenize(query).ToList();

            Assert.True(expected.Equals(tokens[1].Value, System.StringComparison.InvariantCulture));
        }

        [Fact]
        public void LexerTokenPositionsTest()
        {
            var lexer = new Lexer();
            var tokens = Lexer.Tokenize("func(metric.param,'strparam',42,func2('1h24min'))").ToList();

            Assert.Equal(new[] { 0, 4, 5, 17, 18, 28, 29, 31, 32, 37, 38, 47, 48, 0 }, tokens.Select(x => x.StartIndex));
            Assert.Equal(new[] { 3, 4, 16, 17, 27, 28, 30, 31, 36, 37, 46, 47, 48, 0 }, tokens.Select(x => x.EndIndex));
        }

        [Fact]
        public void LexerInvalidCharsTest()
        {
            var lexer = new Lexer();

            Assert.Throws<LexerException>(() => Lexer.Tokenize("func(met:;ric.param").ToList());
            Assert.Throws<LexerException>(() => Lexer.Tokenize("func   :;   (metric.param").ToList());
        }

        [Fact]
        public void LexerPrecedenceTest()
        {
            var lexer = new Lexer();
            var tokens = Lexer.Tokenize("123,123metric,'comma,comma','123','123metric'").ToList();

            Assert.Equal(TokenType.NumberValue, tokens[0].TokenType);
            Assert.Equal(TokenType.Comma, tokens[1].TokenType);
            Assert.Equal(TokenType.Identifier, tokens[2].TokenType);
            Assert.Equal(TokenType.Comma, tokens[3].TokenType);
            Assert.Equal(TokenType.StringValue, tokens[4].TokenType);
            Assert.Equal(TokenType.Comma, tokens[5].TokenType);
            Assert.Equal(TokenType.StringValue, tokens[6].TokenType);
            Assert.Equal(TokenType.Comma, tokens[7].TokenType);
            Assert.Equal(TokenType.StringValue, tokens[8].TokenType);
            Assert.Equal(TokenType.SequenceTerminator, tokens[9].TokenType);
        }

        [Theory]
        [InlineData(@"'adsf\'ads'", @"'adsf\'ads'", @"adsf'ads", false, false)]
        [InlineData(@"'adsf\'\'ads'", @"'adsf\'\'ads'", @"adsf''ads", false, false)]
        [InlineData(@"'\'adsf\'\'ads\''", @"'\'adsf\'\'ads\''", @"'adsf''ads'", false, false)]
        [InlineData(@"'\'adsf\'\'\ads\''", @"'\'adsf\'\'\ads\''", @"'adsf''ads'", false, true)]
        [InlineData(@"'\\\'\\adsf\'\'ads\\\''", @"'\\\'\\adsf\'\'ads\\\''", @"\'\adsf''ads\'", false, false)]
        [InlineData(@"'\\'\\adsf\'\'ads\\\''", "", "", true, false)]
        [InlineData(@"'\''", @"'\''", @"'", false, false)]
        [InlineData(@"'\\'", @"'\\'", @"\", false, false)]
        [InlineData(@"'\t'", @"'\t'", "", false, true)]
        [InlineData(@"'\r'", @"'\r'", "", false, true)]
        [InlineData(@"'\n'", @"'\n'", "", false, true)]
        [InlineData(@"'adsf'ads'", "'adsf'", "adsf", false, false)] // this does not fail in the lexer but will fail in the parser
        public void LexerStringParsingTest(string input, string expectedRaw, string expectedUnescaped, bool expectedTokenizeFail, bool expectedUnescapingFail)
        {
            var lexer = new Lexer();
            if (expectedTokenizeFail)
            {
                Assert.Throws<LexerException>(() => Lexer.Tokenize(input).ToList());
                return;
            }

            var tokens = Lexer.Tokenize(input).ToList();

            Assert.Equal(expectedRaw, tokens[0].Value);
            if (expectedUnescapingFail)
                Assert.Throws<LexerException>(() => tokens[0].Unescaped());
            else
                Assert.Equal(expectedUnescaped, tokens[0].Unescaped());
        }
    }
}