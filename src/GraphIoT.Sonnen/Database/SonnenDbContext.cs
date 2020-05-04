using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Database
{
    public class SonnenDbContext : DbContext, ISonnenDbContext
    {
        public DbSet<SonnenEnergyLowresData> SonnenEnergyLowresDataSet { get; set; } = null!;

        public DbSet<SonnenEnergyMidresData> SonnenEnergyDataSet { get; set; } = null!;

        public DbSet<SonnenChargerLowresData> SonnenChargerLowresDataSet { get; set; } = null!;

        public DbSet<SonnenChargerMidresData> SonnenChargerDataSet { get; set; } = null!;

        public SonnenDbContext(DbContextOptions<SonnenDbContext> options)
            : base(options)
        { }
    }
}