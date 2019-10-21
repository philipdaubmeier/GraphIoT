using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Core.ViewModel
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

        public EventViewModel(DateTime time, string title, string text, IEnumerable<string>? tags)
            => (Time, Title, Text, Tags) = (time, title, text, tags ?? Enumerable.Empty<string>());
    }
}