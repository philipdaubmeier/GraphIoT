using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class CalendarEntry
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CalendarId { get; set; }

        [ForeignKey("CalendarId")]
        public Calendar Calendar { get; set; }

        public bool IsPrivate { get; set; }
        public bool IsFullDay { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime Modified { get; set; }

        [Required, MaxLength(255)]
        public string RecurranceRule { get; set; }
        
        public string Summary { get; set; }

        /// <summary>
        /// BusyStates: 0 = BUSY, 1 = TENTATIVE, 2 = OOF
        /// </summary>
        public int BusyState { get; set; }
    }
}
