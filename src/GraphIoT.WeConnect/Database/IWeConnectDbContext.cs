using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.GraphIoT.WeConnect.Database
{
    public interface IWeConnectDbContext : IDisposable
    {
        DbSet<WeConnectLowresData> WeConnectLowresDataSet { get; set; }

        DbSet<WeConnectMidresData> WeConnectDataSet { get; set; }

        int SaveChanges();
    }
}