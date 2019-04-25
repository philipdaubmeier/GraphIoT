using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.ViessmannHost.Database
{
    public interface IViessmannDbContext : IDisposable
    {
        DbSet<ViessmannHeatingData> ViessmannHeatingTimeseries { get; set; }

        DbSet<ViessmannSolarData> ViessmannSolarTimeseries { get; set; }

        int SaveChanges();
    }
}