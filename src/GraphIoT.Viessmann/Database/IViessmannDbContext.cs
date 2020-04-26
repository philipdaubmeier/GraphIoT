using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Database
{
    public interface IViessmannDbContext : IDisposable
    {
        DbSet<ViessmannHeatingLowresData> ViessmannHeatingLowresTimeseries { get; set; }

        DbSet<ViessmannHeatingMidresData> ViessmannHeatingTimeseries { get; set; }

        DbSet<ViessmannSolarLowresData> ViessmannSolarLowresTimeseries { get; set; }

        DbSet<ViessmannSolarMidresData> ViessmannSolarTimeseries { get; set; }

        int SaveChanges();
    }
}