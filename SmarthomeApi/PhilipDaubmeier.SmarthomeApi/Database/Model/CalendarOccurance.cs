using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.SmarthomeApi.Database.Model
{
    public class CalendarOccurence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid CalendarAppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public CalendarAppointment CalendarAppointment { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public bool IsFullDay { get; set; }

        /// <summary>
        /// If null, use the busy state of the parent appointment.
        /// Is only set if this occurance has a different busy state.
        /// BusyStates: 0 = BUSY, 1 = TENTATIVE, 2 = OOF
        /// </summary>
        public int? ExBusyState { get; set; }

        /// <summary>
        /// If null, use the summary of the parent appointment.
        /// Is only set if this occurance has a different summary.
        /// </summary>
        [MaxLength(120)]
        public string ExSummary { get; set; }

        /// <summary>
        /// If null, use the long location string of the parent appointment.
        /// Is only set if this occurance has a different location.
        /// </summary>
        [MaxLength(80)]
        public string ExLocationLong { get; set; }

        /// <summary>
        /// If null, use the short location string of the parent appointment.
        /// Is only set if this occurance has a different location.
        /// </summary>
        [MaxLength(32)]
        public string ExLocationShort { get; set; }
    }
}
