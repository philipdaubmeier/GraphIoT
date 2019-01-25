using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmarthomeApi.Database.Model
{
    public class PersistenceContext : DbContext
    {
        public DbSet<AuthData> AuthDataSet { get; set; }

        public DbSet<Calendar> Calendars { get; set; }

        public DbSet<CalendarEntry> CalendarEntry { get; set; }

        public PersistenceContext(DbContextOptions<PersistenceContext> options)
            : base(options)
        { }
    }
}
