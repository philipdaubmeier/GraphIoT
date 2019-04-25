using Microsoft.EntityFrameworkCore;
using System;

namespace PhilipDaubmeier.CalendarHost.Database
{
    public interface ICalendarDbContext : IDisposable
    {
        DbSet<Calendar> Calendars { get; set; }

        DbSet<CalendarAppointment> CalendarAppointments { get; set; }

        DbSet<CalendarOccurence> CalendarOccurances { get; set; }

        int SaveChanges();
    }
}