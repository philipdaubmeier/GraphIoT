using PhilipDaubmeier.CompactTimeSeries;
using System.Collections.Generic;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public interface IGraphCollectionViewModel
    {
        string Key { get; }

        TimeSeriesSpan Span { get; set; }

        int GraphCount();

        GraphViewModel Graph(int index);

        IEnumerable<GraphViewModel> Graphs();
    }
}