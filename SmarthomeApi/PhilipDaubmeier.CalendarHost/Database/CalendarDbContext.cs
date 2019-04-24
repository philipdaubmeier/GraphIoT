using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.CalendarHost.Database
{
    public class CalendarDbContext : DbContext
    {
        public DbSet<Calendar> Calendars { get; set; }

        public DbSet<CalendarAppointment> CalendarAppointments { get; set; }

        public DbSet<CalendarOccurence> CalendarOccurances { get; set; }

        public CalendarDbContext(DbContextOptions<CalendarDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}