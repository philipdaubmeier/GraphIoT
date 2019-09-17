using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel
{
    /// <summary>
    /// View model for non-equidistant single occurances like events, incidents etc.
    /// </summary>
    public class EventViewModel
    {
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}