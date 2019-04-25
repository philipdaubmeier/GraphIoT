using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.ViessmannHost.Database
{
    public class ViessmannDbContext : DbContext, IViessmannDbContext
    {
        public DbSet<ViessmannHeatingData> ViessmannHeatingTimeseries { get; set; }

        public DbSet<ViessmannSolarData> ViessmannSolarTimeseries { get; set; }

        public ViessmannDbContext(DbContextOptions<ViessmannDbContext> options)
            : base(options)
        { }
    }
}