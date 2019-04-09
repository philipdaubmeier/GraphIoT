using CompactTimeSeries;
using SmarthomeApi.Model;
using System;

namespace SmarthomeApi.FormatParsers
{
    public static class Base64EventStreamParser
    {
        public static SceneEventStream ToEventStream(this string base64, TimeSeriesSpan span)
        {
            return SceneEventStream.FromByteArray(span, Convert.FromBase64String(base64));
        }

        public static string ToBase64(this SceneEventStream eventstream)
        {
            return Convert.ToBase64String(eventstream.ToByteArray());
        }
    }
}