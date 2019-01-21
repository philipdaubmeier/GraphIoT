using Microsoft.EntityFrameworkCore;

namespace SmarthomeApi.Database.Model
{
    public class Calendar
    {
        public string UUID { get; set; }
        public string Owner { get; set; }
        public DbSet<CalendarEntry> Entries { get; set; }
    }
}
