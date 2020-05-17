using System;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser.Query
{
    public class ParserException : Exception
    {
        public ParserException(string? message) : base(message) { }

        public ParserException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}