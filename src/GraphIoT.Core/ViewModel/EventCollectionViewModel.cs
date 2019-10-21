using PhilipDaubmeier.CompactTimeSeries;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    /// <summary>
    /// View model base class for non-equidistant time series like events, incidents etc.
    /// </summary>
    public abstract class EventCollectionViewModel : IEventCollectionViewModel
    {
        public abstract string Key { get; }

        public TimeSeriesSpan? Span { get; set; } = null;

        public string? Query { get; set; } = null;

        public abstract IEnumerable<EventViewModel> Events { get; }
    }
}