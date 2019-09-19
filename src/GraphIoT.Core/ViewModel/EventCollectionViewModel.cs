using PhilipDaubmeier.CompactTimeSeries;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
{
    /// <summary>
    /// View model base class for non-equidistant time series like events, incidents etc.
    /// </summary>
    public abstract class EventCollectionViewModel : IEventCollectionViewModel
    {
        public virtual string Key { get; }

        public TimeSeriesSpan Span { get; set; }

        public string Query { get; set; }

        public abstract IEnumerable<EventViewModel> Events { get; }
    }
}