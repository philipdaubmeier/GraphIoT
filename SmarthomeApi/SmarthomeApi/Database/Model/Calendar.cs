using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmarthomeApi.Database.Model
{
    public class Calendar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string UUID { get; set; }
        public string Owner { get; set; }
        public DbSet<CalendarEntry> Entries { get; set; }
    }
}
