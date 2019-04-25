using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CalendarHost.Database;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.TokenStore.Database;

namespace PhilipDaubmeier.SmarthomeApi.Database.Model
{
    public class PersistenceContext : DbContext, ITokenStoreDbContext, IDigitalstromDbContext, ICalendarDbContext
    {
        #region ITokenStoreDbContext
        public DbSet<AuthData> AuthDataSet { get; set; }
        #endregion


        #region IDigitalstromDbContext
        public DbSet<DigitalstromZone> DsZones { get; set; }

        public DbSet<DigitalstromZoneSensorData> DsSensorDataSet { get; set; }

        public DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; }

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