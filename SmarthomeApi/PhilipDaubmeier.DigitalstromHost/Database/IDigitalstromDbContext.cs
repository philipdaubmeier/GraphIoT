using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public interface IDigitalstromDbContext : IDisposable
    {
        DbSet<DigitalstromZone> DsZones { get; set; }

        DbSet<DigitalstromZoneSensorData> DsSensorDataSet { get; set; }

        DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; }

        DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; }

        int SaveChanges();
    }
}