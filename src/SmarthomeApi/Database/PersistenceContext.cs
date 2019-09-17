using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CalendarHost.Database;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.NetatmoHost.Database;
using PhilipDaubmeier.SonnenHost.Database;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.ViessmannHost.Database;

namespace PhilipDaubmeier.SmarthomeApi.Database
{
    public class PersistenceContext : DbContext, ITokenStoreDbContext, IDigitalstromDbContext, ICalendarDbContext, INetatmoDbContext, IViessmannDbContext, ISonnenDbContext
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


        #region ICalendarDbContext
        public DbSet<Calendar> Calendars { get; set; }

        public DbSet<CalendarAppointment> CalendarAppointments { get; set; }

        public DbSet<CalendarOccurence> CalendarOccurances { get; set; }
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