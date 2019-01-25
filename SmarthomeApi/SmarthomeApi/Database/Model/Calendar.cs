using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmarthomeApi.Database.Model
{
    public class Calendar
    {
        [Key]
        public Guid Id { get; set; }

        public string Owner { get; set; }
        public ICollection<CalendarEntry> Entries { get; set; }
    }
}
