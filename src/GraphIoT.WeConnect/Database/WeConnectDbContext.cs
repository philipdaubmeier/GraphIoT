using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.GraphIoT.WeConnect.Database
{
    public class WeConnectDbContext : DbContext, IWeConnectDbContext
    {
        public DbSet<WeConnectLowresData> WeConnectLowresDataSet { get; set; } = null!;

        public DbSet<WeConnectMidresData> WeConnectDataSet { get; set; } = null!;

        public WeConnectDbContext(DbContextOptions<WeConnectDbContext> options)
            : base(options)
        { }
    }
}