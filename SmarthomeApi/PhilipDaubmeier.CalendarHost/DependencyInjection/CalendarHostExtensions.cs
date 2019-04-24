using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.CalendarHost.Database;
using System;

namespace PhilipDaubmeier.CalendarHost.DependencyInjection
{
    public static class CalendarHostExtensions
    {
        public static IServiceCollection AddCalendarHost(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig)
        {
            serviceCollection.AddDbContext<CalendarDbContext>(dbConfig);
            return serviceCollection;
        }
    }
}