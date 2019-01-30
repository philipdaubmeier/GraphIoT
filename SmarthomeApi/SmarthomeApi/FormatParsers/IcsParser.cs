using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using SmarthomeApi.Database.Model;
using SmarthomeApi.FormatParsers.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmarthomeApi.FormatParsers
{
    public class IcsParser
    {
        private PersistenceContext _dbContext;
        private string _owner;

        public IcsParser(PersistenceContext databaseContext, string ownerEmail)
        {
            _dbContext = databaseContext;
            _owner = ownerEmail;
        }
        
        public void Parse(TextReader reader)
        {
            var calendar = Ical.Net.Calendar.Load(reader);
            StoreCalendarEntries(_dbContext, calendar);
        }

        private int ToBusyState(string icsBusyString)
        {
            if (string.IsNullOrEmpty(icsBusyString))
                return 0;

            switch (icsBusyString.Trim().ToUpperInvariant())
            {
                case "TENTATIVE": return 1;
                case "OOF": return 2;
                default: return 0;
            }
        }

        private void StoreCalendarEntries(PersistenceContext db, Ical.Net.Calendar calendar)
        {
            var calendarIdStr = calendar.Properties.FirstOrDefault(p => p.Name.Equals("X-WR-RELCALID"))?.Value as string;
            var owner = calendar.Properties.FirstOrDefault(p => p.Name.Equals("X-OWNER"))?.Value as string;

            if (!Guid.TryParse(calendarIdStr, out Guid calendarGuid))
                return;

            var dbCalendar = db.Calendars.Where(x => x.Id == calendarGuid).FirstOrDefault();
            if (dbCalendar == null)
                db.Calendars.Add(dbCalendar = new Calendar() { Id = calendarGuid, Owner = owner });

            if (!DateTime.TryParseExact(calendar.Properties.FirstOrDefault(p => p.Name.Equals("X-CLIPSTART"))?.Value as string, "yyyyMMdd'T'HHmmss'Z'",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateFrom))
                return;

            if (!DateTime.TryParseExact(calendar.Properties.FirstOrDefault(p => p.Name.Equals("X-CLIPEND"))?.Value as string, "yyyyMMdd'T'HHmmss'Z'",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime dateTo))
                return;

            var entries = calendar.GetOccurrences(dateFrom, dateTo)
            .Select(o => o.Source).Cast<CalendarEvent>().Distinct().ToList();

            // Load and delete all occurences in the given date range
            var oldOccurences = db.CalendarOccurances.Where(x => x.StartTime < dateTo && x.EndTime > dateFrom).ToList();
            db.CalendarOccurances.RemoveRange(oldOccurences);

            foreach (var entryGrouping in entries.GroupBy(o => GuidUtility.FromIcsUID(o.Uid)))
            {
                var entry = entryGrouping.FirstOrDefault(x => x.RecurrenceId == null);
                if (entry == null)
                    continue;

                var baseOccurences = entryGrouping.Where(e => e.RecurrenceId == null).SelectMany(e => e.GetOccurrences(dateFrom, dateTo));
                var exceptOccurences = entryGrouping.Where(e => e.RecurrenceId != null).ToDictionary(e => e.RecurrenceId.AsUtc);
                var allOccurences = baseOccurences.Where(o => !exceptOccurences.ContainsKey(o.Period.StartTime.AsUtc))
                        .Union(exceptOccurences.SelectMany(e => e.Value.GetOccurrences(dateFrom, dateTo)));
                
                var dbAppointment = CreateOrUpdateAppointment(db, dbCalendar, entry);

                // Create new occurences
                foreach (var occurence in allOccurences)
                    CreateOccurence(db, dbAppointment, occurence);
            }

            db.SaveChanges();
        }

        private CalendarAppointment CreateOrUpdateAppointment(PersistenceContext db, Calendar dbCalendar, CalendarEvent entry)
        {
            var summary = string.IsNullOrEmpty(entry.Summary) ? "" : entry.Summary.Length <= 120 ? entry.Summary : entry.Summary.Substring(0, 117) + "...";
            var busyStr = entry.Properties.FirstOrDefault(p => p.Name.Equals("X-MICROSOFT-CDO-BUSYSTATUS"))?.Value as string;
            
            var guid = GuidUtility.FromIcsUID(entry.Uid);
            var isPrivate = entry.Class == null ? false : !entry.Class.Equals("PUBLIC", StringComparison.InvariantCultureIgnoreCase);
            var busyState = ToBusyState(busyStr);
            
            if (dbCalendar.Appointments == null)
                dbCalendar.Appointments = new List<CalendarAppointment>();
            
            var dbEntry = db.CalendarAppointments.Where(x => x.Id == guid).FirstOrDefault();
            if (dbEntry == null)
            {
                dbEntry = new CalendarAppointment();
                dbEntry.Id = guid;
                dbEntry.Calendar = dbCalendar;

                db.CalendarAppointments.Add(dbEntry);
                dbCalendar.Appointments.Add(dbEntry);
            }

            dbEntry.IsPrivate = isPrivate;
            dbEntry.Summary = summary;
            dbEntry.BusyState = busyState;

            return dbEntry;
        }

        private void CreateOccurence(PersistenceContext db, CalendarAppointment dbAppointment, Occurrence occurence)
        {
            if (dbAppointment.Occurences == null)
                dbAppointment.Occurences = new List<CalendarOccurence>();
            
            var dbOccurence = new CalendarOccurence();
            db.CalendarOccurances.Add(dbOccurence);
            dbAppointment.Occurences.Add(dbOccurence);

            dbOccurence.IsFullDay = (occurence.Source as CalendarEvent)?.IsAllDay ?? false;
            dbOccurence.StartTime = occurence.Period.StartTime?.Value ?? DateTime.MinValue;
            dbOccurence.EndTime = occurence.Period.EndTime?.Value ?? DateTime.MinValue;
        }
    }
}
