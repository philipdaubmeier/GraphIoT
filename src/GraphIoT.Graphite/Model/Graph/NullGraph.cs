using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class NullGraph : GraphiteGraph
    {
        public override string Name => string.Empty;
        public override DateTime Begin => DateTime.MinValue;
        public override TimeSpan Spacing => TimeSpan.FromSeconds(1);

        public override IEnumerable<double?> Points
        {
            get { yield break; }
        }

        public NullGraph() { }
    }
}