using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Database
{
    public class DigitalstromDbContext : DbContext, IDigitalstromDbContext
    {
        public DbSet<DigitalstromZone> DsZones { get; set; } = null!;

        public DbSet<DigitalstromCircuit> DsCircuits { get; set; } = null!;

        public DbSet<DigitalstromZoneSensorLowresData> DsSensorLowresDataSet { get; set; } = null!;

        public DbSet<DigitalstromZoneSensorMidresData> DsSensorDataSet { get; set; } = null!;

        public DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; } = null!;

        public DbSet<DigitalstromEnergyLowresData> DsEnergyLowresDataSet { get; set; } = null!;

        public DbSet<DigitalstromEnergyMidresData> DsEnergyMidresDataSet { get; set; } = null!;

        public DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; } = null!;

        public DigitalstromDbContext(DbContextOptions<DigitalstromDbContext> options)
            : base(options)
        { }
    }
}