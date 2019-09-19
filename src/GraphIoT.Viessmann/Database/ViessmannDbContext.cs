using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Database
{
    public class ViessmannDbContext : DbContext, IViessmannDbContext
    {
        public DbSet<ViessmannHeatingLowresData> ViessmannHeatingLowresTimeseries { get; set; }

        public DbSet<ViessmannHeatingMidresData> ViessmannHeatingTimeseries { get; set; }

        public DbSet<ViessmannSolarLowresData> ViessmannSolarLowresTimeseries { get; set; }

        public DbSet<ViessmannSolarMidresData> ViessmannSolarTimeseries { get; set; }

        public ViessmannDbContext(DbContextOptions<ViessmannDbContext> options)
            : base(options)
        { }
    }
}