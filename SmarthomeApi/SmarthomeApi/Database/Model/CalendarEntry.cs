using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class CalendarEntry
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string UID { get; set; }

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
