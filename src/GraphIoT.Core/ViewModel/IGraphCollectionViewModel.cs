using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Aggregation;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    public interface IGraphCollectionViewModel
    {
        string Key { get; }

        TimeSeriesSpan Span { get; set; }

        int GraphCount();

        IEnumerable<string> GraphKeys();

        GraphViewModel Graph(int index);

        IEnumerable<GraphViewModel> Graphs();

        Aggregator AggregatorFunction { get; set; }

        double CorrectionFactor { get; set; }

        double CorrectionOffset { get; set; }
    }
}