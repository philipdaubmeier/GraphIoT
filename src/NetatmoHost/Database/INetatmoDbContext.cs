using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.NetatmoHost.Database
{
    public interface INetatmoDbContext : IDisposable
    {
        DbSet<NetatmoModuleMeasure> NetatmoModuleMeasures { get; set; }

        DbSet<NetatmoMeasureLowresData> NetatmoMeasureLowresDataSet { get; set; }

        DbSet<NetatmoMeasureMidresData> NetatmoMeasureDataSet { get; set; }

        int SaveChanges();
    }
}