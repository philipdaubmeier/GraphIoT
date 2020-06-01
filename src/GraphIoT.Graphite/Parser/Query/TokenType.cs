namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public enum TokenType
    {
        Identifier,
        Comma,
        OpenParanthesis,
        CloseParanthesis,
        StringValue,
        NumberValue,
        TrueLiteral,
        FalseLiteral,
        SequenceTerminator
    }
}