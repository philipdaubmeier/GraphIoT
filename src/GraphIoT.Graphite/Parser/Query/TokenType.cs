namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public enum TokenType
    {
        Identifier,
        Comma,
        OpenParanthesis,
        CloseParanthesis,
        OpenCurlyBrackets,
        CloseCurlyBrackets,
        StringValue,
        NumberValue,
        TrueLiteral,
        FalseLiteral,
        SequenceTerminator
    }
}