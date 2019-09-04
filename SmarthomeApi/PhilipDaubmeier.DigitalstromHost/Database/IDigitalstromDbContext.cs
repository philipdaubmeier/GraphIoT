using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public interface IDigitalstromDbContext : IDisposable
    {
        DbSet<DigitalstromZone> DsZones { get; set; }

        DbSet<DigitalstromCircuit> DsCircuits { get; set; }

        DbSet<DigitalstromZoneSensorLowresData> DsSensorLowresDataSet { get; set; }

        DbSet<DigitalstromZoneSensorMidresData> DsSensorDataSet { get; set; }

        DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; }

        DbSet<DigitalstromEnergyLowresData> DsEnergyLowresDataSet { get; set; }

        DbSet<DigitalstromEnergyMidresData> DsEnergyMidresDataSet { get; set; }

        DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; }

        int SaveChanges();
    }
}