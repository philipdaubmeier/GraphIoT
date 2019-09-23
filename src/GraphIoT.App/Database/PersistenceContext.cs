using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Sonnen.Database;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using PhilipDaubmeier.TokenStore.Database;

namespace PhilipDaubmeier.GraphIoT.App.Database
{
    public class PersistenceContext : DbContext, ITokenStoreDbContext, IDigitalstromDbContext, INetatmoDbContext, IViessmannDbContext, ISonnenDbContext
    {
        #region ITokenStoreDbContext
        public DbSet<AuthData> AuthDataSet { get; set; }
        #endregion


        #region IDigitalstromDbContext
        public DbSet<DigitalstromZone> DsZones { get; set; }

        public DbSet<DigitalstromCircuit> DsCircuits { get; set; }

        public DbSet<DigitalstromZoneSensorLowresData> DsSensorLowresDataSet { get; set; }

        public DbSet<DigitalstromZoneSensorMidresData> DsSensorDataSet { get; set; }

        public DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; }

        public DbSet<DigitalstromEnergyLowresData> DsEnergyLowresDataSet { get; set; }

        public DbSet<DigitalstromEnergyMidresData> DsEnergyMidresDataSet { get; set; }

        public DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; }
        #endregion


        #region INetatmoDbContext
        public DbSet<NetatmoModuleMeasure> NetatmoModuleMeasures { get; set; }

        public DbSet<NetatmoMeasureLowresData> NetatmoMeasureLowresDataSet { get; set; }

        public DbSet<NetatmoMeasureMidresData> NetatmoMeasureDataSet { get; set; }
        #endregion


        #region IViessmannDbContext
        public DbSet<ViessmannHeatingLowresData> ViessmannHeatingLowresTimeseries { get; set; }

        public DbSet<ViessmannHeatingMidresData> ViessmannHeatingTimeseries { get; set; }

        public DbSet<ViessmannSolarLowresData> ViessmannSolarLowresTimeseries { get; set; }

        public DbSet<ViessmannSolarMidresData> ViessmannSolarTimeseries { get; set; }
        #endregion


        #region ISonnenDbContext
        public DbSet<SonnenEnergyLowresData> SonnenEnergyLowresDataSet { get; set; }

        public DbSet<SonnenEnergyMidresData> SonnenEnergyDataSet { get; set; }
        #endregion


        public PersistenceContext(DbContextOptions<PersistenceContext> options)
            : base(options)
        { }
    }
}