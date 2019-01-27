using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class CalendarOccurence
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid CalendarAppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public CalendarAppointment CalendarAppointment { get; set; }
        
        public bool IsFullDay { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
