using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.SonnenHost.Database
{
    public interface ISonnenDbContext : IDisposable
    {
        DbSet<SonnenEnergyData> SonnenEnergyDataSet { get; set; }

        int SaveChanges();
    }
}