using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.CalendarHost.Database;
using System;

namespace PhilipDaubmeier.CalendarHost.DependencyInjection
{
    public static class CalendarHostExtensions
    {
        public static IServiceCollection AddCalendarHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig) where TDbContext : DbContext, ICalendarDbContext
        {
            serviceCollection.AddDbContext<ICalendarDbContext, TDbContext>(dbConfig);
            return serviceCollection;
        }
    }
}