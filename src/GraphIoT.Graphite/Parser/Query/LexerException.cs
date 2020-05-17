using System;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public class LexerException : Exception
    {
        public LexerException(string? message) : base(message) { }

        public LexerException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}