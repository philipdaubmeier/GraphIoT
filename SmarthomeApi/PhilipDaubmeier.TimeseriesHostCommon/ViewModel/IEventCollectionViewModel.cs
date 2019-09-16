using PhilipDaubmeier.CompactTimeSeries;
using System.Collections.Generic;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    public interface IEventCollectionViewModel
    {
        string Key { get; }

        TimeSeriesSpan Span { get; set; }

        string Query { get; set; }

        IEnumerable<EventViewModel> Events { get; }
    }
}