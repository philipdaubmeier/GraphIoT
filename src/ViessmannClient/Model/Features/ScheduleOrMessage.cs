using System.Collections.Generic;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class ScheduleOrMessage
    {
        public Schedule? Schedule { get; set; }
        public List<Message>? Messages { get; set; }
    }
}