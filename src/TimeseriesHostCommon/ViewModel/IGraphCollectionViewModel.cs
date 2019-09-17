using PhilipDaubmeier.CompactTimeSeries;
using System.Collections.Generic;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public enum Aggregator
    {
        Default,
        Minimum,
        Maximum,
        Average,
        Sum
    }

    public interface IGraphCollectionViewModel
    {
        string Key { get; }

        TimeSeriesSpan Span { get; set; }

        int GraphCount();

        GraphViewModel Graph(int index);

        IEnumerable<GraphViewModel> Graphs();

        Aggregator AggregatorFunction { get; set; }

        decimal CorrectionFactor { get; set; }

        decimal CorrectionOffset { get; set; }
    }
}