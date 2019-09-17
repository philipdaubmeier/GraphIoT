using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhilipDaubmeier.CalendarHost.Database
{
    public class Calendar
    {
        [Key]
        public Guid Id { get; set; }

        public string Owner { get; set; }
        public ICollection<CalendarAppointment> Appointments { get; set; }
    }
}
