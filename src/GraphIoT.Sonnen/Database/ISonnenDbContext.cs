using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Database
{
    public interface ISonnenDbContext : IDisposable
    {
        DbSet<SonnenEnergyLowresData> SonnenEnergyLowresDataSet { get; set; }

        DbSet<SonnenEnergyMidresData> SonnenEnergyDataSet { get; set; }

        DbSet<SonnenChargerLowresData> SonnenChargerLowresDataSet { get; set; }

        DbSet<SonnenChargerMidresData> SonnenChargerDataSet { get; set; }

        int SaveChanges();
    }
}