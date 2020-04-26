using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.TokenStore.Database;

namespace PhilipDaubmeier.DigitalstromTimeSeriesApi.Database
{
    public class IntegrationTestDbContext : DbContext, ITokenStoreDbContext, IDigitalstromDbContext
    {
        #region ITokenStoreDbContext
        public DbSet<AuthData> AuthDataSet { get; set; } = null!;
        #endregion

        #region IDigitalstromDbContext
        public DbSet<DigitalstromZone> DsZones { get; set; } = null!;

        public DbSet<DigitalstromCircuit> DsCircuits { get; set; } = null!;

        public DbSet<DigitalstromZoneSensorLowresData> DsSensorLowresDataSet { get; set; } = null!;

        public DbSet<DigitalstromZoneSensorMidresData> DsSensorDataSet { get; set; } = null!;

        public DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; } = null!;

        public DbSet<DigitalstromEnergyLowresData> DsEnergyLowresDataSet { get; set; } = null!;

        public DbSet<DigitalstromEnergyMidresData> DsEnergyMidresDataSet { get; set; } = null!;

        public DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; } = null!;
        #endregion

        public IntegrationTestDbContext(DbContextOptions<IntegrationTestDbContext> options)
            : base(options)
        { }
    }
}