using PhilipDaubmeier.CompactTimeSeries;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    public enum Aggregator
    {
        Default,
        Average,
        AverageZero,
        Median,
        Sum,
        Minimum,
        Maximum,
        Diff,
        Stddev,
        Count,
        Range,
        Multiply,
        Last
    }

    public interface IGraphCollectionViewModel
    {
        string Key { get; }

        TimeSeriesSpan Span { get; set; }

        int GraphCount();

        IEnumerable<string> GraphKeys();

        GraphViewModel Graph(int index);

        IEnumerable<GraphViewModel> Graphs();

        Aggregator AggregatorFunction { get; set; }

        decimal CorrectionFactor { get; set; }

        decimal CorrectionOffset { get; set; }
    }
}