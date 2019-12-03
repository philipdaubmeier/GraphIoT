using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Database
{
    public class ViessmannDbContext : DbContext, IViessmannDbContext
    {
        public DbSet<ViessmannHeatingLowresData> ViessmannHeatingLowresTimeseries { get; set; } = null!;

        public DbSet<ViessmannHeatingMidresData> ViessmannHeatingTimeseries { get; set; } = null!;

        public DbSet<ViessmannSolarLowresData> ViessmannSolarLowresTimeseries { get; set; } = null!;

        public DbSet<ViessmannSolarMidresData> ViessmannSolarTimeseries { get; set; } = null!;

        public ViessmannDbContext(DbContextOptions<ViessmannDbContext> options)
            : base(options)
        { }
    }
}