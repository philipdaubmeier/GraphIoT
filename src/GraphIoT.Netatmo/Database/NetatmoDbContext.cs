using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Database
{
    public class NetatmoDbContext : DbContext, INetatmoDbContext
    {
        public DbSet<NetatmoModuleMeasure> NetatmoModuleMeasures { get; set; } = null!;

        public DbSet<NetatmoMeasureLowresData> NetatmoMeasureLowresDataSet { get; set; } = null!;

        public DbSet<NetatmoMeasureMidresData> NetatmoMeasureDataSet { get; set; } = null!;

        public NetatmoDbContext(DbContextOptions<NetatmoDbContext> options)
            : base(options)
        { }
    }
}