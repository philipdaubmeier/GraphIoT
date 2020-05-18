using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public abstract class GraphiteGraph : IGraphiteGraph
    {
        public abstract string Name { get; }
        public abstract DateTime Begin { get; }
        public abstract TimeSpan Spacing { get; }
        public abstract IEnumerable<double?> Points { get; }

        public IEnumerable<dynamic?[]> TimestampedPoints()
        {
            var timestamp = new DateTimeOffset(Begin.ToUniversalTime()).ToUnixTimeSeconds();
            foreach (var point in Points)
            {
                yield return new dynamic?[] { point, timestamp };
                timestamp += (int)Spacing.TotalSeconds;
            }
        }
    }
}