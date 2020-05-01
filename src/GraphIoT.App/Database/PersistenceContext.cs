using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Sonnen.Database;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using PhilipDaubmeier.GraphIoT.WeConnect.Database;
using PhilipDaubmeier.TokenStore.Database;

namespace PhilipDaubmeier.GraphIoT.App.Database
{
    public class PersistenceContext : DbContext, ITokenStoreDbContext, IDigitalstromDbContext, INetatmoDbContext, IViessmannDbContext, ISonnenDbContext, IWeConnectDbContext
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


        #region INetatmoDbContext
        public DbSet<NetatmoModuleMeasure> NetatmoModuleMeasures { get; set; } = null!;

        public DbSet<NetatmoMeasureLowresData> NetatmoMeasureLowresDataSet { get; set; } = null!;

        public DbSet<NetatmoMeasureMidresData> NetatmoMeasureDataSet { get; set; } = null!;
        #endregion


        #region IViessmannDbContext
        public DbSet<ViessmannHeatingLowresData> ViessmannHeatingLowresTimeseries { get; set; } = null!;

        public DbSet<ViessmannHeatingMidresData> ViessmannHeatingTimeseries { get; set; } = null!;

        public DbSet<ViessmannSolarLowresData> ViessmannSolarLowresTimeseries { get; set; } = null!;

        public DbSet<ViessmannSolarMidresData> ViessmannSolarTimeseries { get; set; } = null!;
        #endregion


        #region ISonnenDbContext
        public DbSet<SonnenEnergyLowresData> SonnenEnergyLowresDataSet { get; set; } = null!;

        public DbSet<SonnenEnergyMidresData> SonnenEnergyDataSet { get; set; } = null!;
        #endregion


        #region IWeConnectDbContext
        public DbSet<WeConnectLowresData> WeConnectLowresDataSet { get; set; } = null!;

        public DbSet<WeConnectMidresData> WeConnectDataSet { get; set; } = null!;
        #endregion


        public PersistenceContext(DbContextOptions<PersistenceContext> options)
            : base(options)
        { }
    }
}