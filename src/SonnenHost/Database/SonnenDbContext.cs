using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.SonnenHost.Database
{
    public class SonnenDbContext : DbContext, ISonnenDbContext
    {
        public DbSet<SonnenEnergyLowresData> SonnenEnergyLowresDataSet { get; set; }

        public DbSet<SonnenEnergyMidresData> SonnenEnergyDataSet { get; set; }

        public SonnenDbContext(DbContextOptions<SonnenDbContext> options)
            : base(options)
        { }
    }
}