using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.CalendarHost.Database
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

        /// <summary>
        /// BusyStates: 0 = BUSY, 1 = TENTATIVE, 2 = OOF
        /// </summary>
        [Required]
        public int BusyState { get; set; }

        [Required, MaxLength(120)]
        public string Summary { get; set; }

        [Required, MaxLength(80)]
        public string LocationLong { get; set; }

        [Required, MaxLength(32)]
        public string LocationShort { get; set; }
    }
}
