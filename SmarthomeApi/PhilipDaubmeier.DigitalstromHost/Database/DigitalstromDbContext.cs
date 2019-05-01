﻿using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.DigitalstromHost.Database
{
    public class DigitalstromDbContext : DbContext, IDigitalstromDbContext
    {
        public DbSet<DigitalstromZone> DsZones { get; set; }

        public DbSet<DigitalstromZoneSensorData> DsSensorDataSet { get; set; }

        public DbSet<DigitalstromSceneEventData> DsSceneEventDataSet { get; set; }

        public DbSet<DigitalstromEnergyHighresData> DsEnergyHighresDataSet { get; set; }
        
        public DigitalstromDbContext(DbContextOptions<DigitalstromDbContext> options)
            : base(options)
        { }
    }
}