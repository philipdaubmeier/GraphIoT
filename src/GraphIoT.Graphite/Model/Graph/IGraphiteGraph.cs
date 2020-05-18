using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public interface IGraphiteGraph
    {
        string Name { get; }
        DateTime Begin { get; }
        TimeSpan Spacing { get; }
        IEnumerable<double?> Points { get; }

        IEnumerable<dynamic?[]> TimestampedPoints();
    }
}