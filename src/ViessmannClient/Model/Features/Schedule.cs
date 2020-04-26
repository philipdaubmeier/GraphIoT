using System.Collections.Generic;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class Schedule
    {
        public List<ScheduleEntry>? Mon { get; set; }
        public List<ScheduleEntry>? Tue { get; set; }
        public List<ScheduleEntry>? Wed { get; set; }
        public List<ScheduleEntry>? Thu { get; set; }
        public List<ScheduleEntry>? Fri { get; set; }
        public List<ScheduleEntry>? Sat { get; set; }
        public List<ScheduleEntry>? Sun { get; set; }
    }

    public class ScheduleEntry
    {
        public string? Start { get; set; }
        public string? End { get; set; }
        public string? Mode { get; set; }
        public int? Position { get; set; }
    }
}