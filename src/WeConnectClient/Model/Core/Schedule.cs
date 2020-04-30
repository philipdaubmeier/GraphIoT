using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.Core
{
    public class Schedule
    {
        public int Type { get; set; }
        public TimeOfDay Start { get; set; } = new TimeOfDay();
        public TimeOfDay End { get; set; } = new TimeOfDay();
        public int? Index { get; set; }
        public List<string> Daypicker { get; set; } = new List<string>();
        public string StartDateActive { get; set; } = string.Empty;
        public string EndDateActive { get; set; } = string.Empty;
    }

    public class TimeOfDay
    {
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
    }
}