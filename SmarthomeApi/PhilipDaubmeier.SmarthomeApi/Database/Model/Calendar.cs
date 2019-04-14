using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhilipDaubmeier.SmarthomeApi.Database.Model
{
    public class Calendar
    {
        [Key]
        public Guid Id { get; set; }

        public string Owner { get; set; }
        public ICollection<CalendarAppointment> Appointments { get; set; }
    }
}
