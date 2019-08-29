using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CalendarHost.Database;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.SonnenHost.Database;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.ViessmannHost.Database;

namespace PhilipDaubmeier.SmarthomeApi.Database
{
    public class PersistenceContext : DbContext, ITokenStoreDbContext, IDigitalstromDbContext, ICalendarDbContext, IViessmannDbContext, ISonnenDbContext
    {
        #region ITokenStoreDbContext
        public DbSet<AuthData> AuthDataSet { get; set; }
        #endregion


        #region IDigitalstromDbContext
        public DbSet<DigitalstromZone> DsZones { get; set; }

        public DbSet<DigitalstromCircuit> DsCircuits { get; set; }

        public DbSet<DigitalstromZoneSensorData> DsSensorDataSet { get; set; }

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


        #region IViessmannDbContext
        public DbSet<ViessmannHeatingData> ViessmannHeatingTimeseries { get; set; }

        public DbSet<ViessmannSolarData> ViessmannSolarTimeseries { get; set; }
        #endregion


        #region ISonnenDbContext
        public DbSet<SonnenEnergyData> SonnenEnergyDataSet { get; set; }
        #endregion


        public PersistenceContext(DbContextOptions<PersistenceContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ViessmannSolarData>()
                .HasIndex(d => d.Day)
                .IsUnique();
        }
    }
}