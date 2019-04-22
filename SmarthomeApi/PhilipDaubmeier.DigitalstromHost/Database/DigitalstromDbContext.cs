using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromDbContext : DbContext
    {
        public DbSet<DigitalstromZone> DsZones { get; set; }

        public DbSet<DigitalstromZoneSensorData> DsSensorDataSet { get; set; }

        public DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; }

        public DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; }
        
        public DigitalstromDbContext(DbContextOptions<DigitalstromDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}