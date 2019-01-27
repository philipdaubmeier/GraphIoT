using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class CalendarAppointment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CalendarId { get; set; }

        [ForeignKey("CalendarId")]
        public Calendar Calendar { get; set; }

        public ICollection<CalendarOccurence> Occurences { get; set; }

        [Required]
        public bool IsPrivate { get; set; }
        
        [Required, MaxLength(120)]
        public string Summary { get; set; }

        /// <summary>
        /// BusyStates: 0 = BUSY, 1 = TENTATIVE, 2 = OOF
        /// </summary>
        [Required]
        public int BusyState { get; set; }
    }
}
