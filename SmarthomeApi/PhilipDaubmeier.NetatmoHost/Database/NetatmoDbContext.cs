using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.NetatmoHost.Database
{
    public class NetatmoDbContext : DbContext, INetatmoDbContext
    {
        public DbSet<NetatmoModuleMeasure> NetatmoModuleMeasures { get; set; }

        public DbSet<NetatmoMeasureLowresData> NetatmoMeasureLowresDataSet { get; set; }

        public DbSet<NetatmoMeasureMidresData> NetatmoMeasureDataSet { get; set; }

        public NetatmoDbContext(DbContextOptions<NetatmoDbContext> options)
            : base(options)
        { }
    }
}