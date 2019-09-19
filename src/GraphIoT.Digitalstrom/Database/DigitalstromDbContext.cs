using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Database
{
    public class DigitalstromDbContext : DbContext, IDigitalstromDbContext
    {
        public DbSet<DigitalstromZone> DsZones { get; set; }

        public DbSet<DigitalstromCircuit> DsCircuits { get; set; }

        public DbSet<DigitalstromZoneSensorLowresData> DsSensorLowresDataSet { get; set; }

        public DbSet<DigitalstromZoneSensorMidresData> DsSensorDataSet { get; set; }

        public DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; }

        public DbSet<DigitalstromEnergyLowresData> DsEnergyLowresDataSet { get; set; }

        public DbSet<DigitalstromEnergyMidresData> DsEnergyMidresDataSet { get; set; }

        public DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; }
        
        public DigitalstromDbContext(DbContextOptions<DigitalstromDbContext> options)
            : base(options)
        { }
    }
}