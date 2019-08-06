using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.SonnenHost.Database
{
    public class SonnenDbContext : DbContext, ISonnenDbContext
    {
        public DbSet<SonnenEnergyData> SonnenEnergyDataSet { get; set; }

        public SonnenDbContext(DbContextOptions<SonnenDbContext> options)
            : base(options)
        { }
    }
}