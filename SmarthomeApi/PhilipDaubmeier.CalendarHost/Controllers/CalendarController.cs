using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhilipDaubmeier.CalendarHost.Database;
using PhilipDaubmeier.CalendarHost.FormatParsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.CalendarHost.Controllers
{
    [Produces("application/json")]
    [Route("api/calendars")]
    public class CalendarController : Controller
    {
        private readonly CalendarDbContext _dbContext;
        public CalendarController(CalendarDbContext databaseContext)
        {
            _dbContext = databaseContext;
        }

        // POST: api/calendars/{calendarid}/update
        [HttpPost("{calendarid}/update")]
        public ActionResult Post()
        {
            var parser = new IcsParser(_dbContext, "philip.daubmeier@audi.de");
            using (var reader = new StreamReader(Request.Body))
            {
                parser.Parse(reader);
            }

            return StatusCode(200);
        }

        // GET: api/calendars/transform
        [HttpGet("transform")]
        public async Task<JsonResult> Transform()
        {
            Ical.Net.Calendar calendar = null;

            using (var reader = new StreamReader(Request.Body))
            {
                await Task.Factory.StartNew(() =>
                {
                    calendar = Ical.Net.Calendar.Load(reader);
                });
            }

            var entries = calendar.GetOccurrences(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30))
                .Select(o => o.Source).Cast<CalendarEvent>().Distinct().ToList();

            return Json(new
            {
                events = entries.Select(x => new
                {
                    id = x.Uid,
                    summary = x.Summary.Length < 120 ? x.Summary : x.Summary.Substring(0, 120),
                    busy = (x.Properties.FirstOrDefault(p => p.Name.Equals("X-MICROSOFT-CDO-BUSYSTATUS"))?.Value as string)?.ToLowerInvariant() ?? "busy",
                    @private = !x.Class.Equals("PUBLIC", StringComparison.InvariantCultureIgnoreCase),
                    periods = x.GetOccurrences(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30)).Select(y => new
                    {
                        start = y.Period.StartTime?.Value.ToLocalTime().ToString("o", CultureInfo.InvariantCulture),
                        end = y.Period.EndTime?.Value.ToLocalTime().ToString("o", CultureInfo.InvariantCulture),
                        allday = x.IsAllDay
                    })
                })
            });
        }

        // GET api/calendars/{calendarid}/years/{year}/calendarweeks/{week}
        [HttpGet("{calendarid}/years/{year}/calendarweeks/{week}")]
        public ActionResult Get([FromRoute] string calendarid, [FromRoute] string year, [FromRoute] string week, [FromQuery] string password)
        {
            if (password != "9bb04cf87b69e851")
                return StatusCode(403);

            if (!Guid.TryParse(calendarid, out Guid calendarGuid)
                || !int.TryParse(year, out int yearNum)
                || !int.TryParse(week, out int weekNum))
                return StatusCode(404);

            var startTime = CalendarWeekUtility.FirstDateOfWeek(yearNum, weekNum).ToUniversalTime();
            var endTime = startTime.AddDays(7);

            var calendar = _dbContext.Calendars.Where(cal => cal.Id == calendarGuid).FirstOrDefault();
            var entries = _dbContext.CalendarOccurances
                .Include(occur => occur.CalendarAppointment)
                .Where(occur => occur.CalendarAppointment.CalendarId == calendarGuid)
                .Where(occur => occur.EndTime >= startTime)
                .Where(occur => occur.StartTime < endTime)
                .ToList().GroupBy(x => x.CalendarAppointment)
                .Where(group => group.Count() > 0);

            Func<int?, string> toBusy = b => b == 1 ? "tentative" : b == 2 ? "out_of_office" : "busy";

            return Json(new
            {
                result = new
                {
                    calendar = new
                    {
                        id = calendar.Id,
                        owner = calendar.Owner
                    },
                    filter_period = new
                    {
                        start = startTime.ToLocalTime().ToString("o", CultureInfo.InvariantCulture),
                        end = endTime.ToLocalTime().ToString("o", CultureInfo.InvariantCulture)
                    },
                    events = entries.Select(x => new
                    {
                        id = x.Key?.Id,
                        summary = x.Key?.Summary,
                        @private = x.Key?.IsPrivate,
                        occurences = x.Select(o => new
                        {
                            start = o.StartTime.ToLocalTime().ToString("o", CultureInfo.InvariantCulture),
                            end = o.EndTime.ToLocalTime().ToString("o", CultureInfo.InvariantCulture),
                            allday = o.IsFullDay,
                            busy = toBusy(o.ExBusyState ?? x.Key?.BusyState),
                            location = o.ExLocationLong ?? x.Key?.LocationLong,
                            location_short = o.ExLocationShort ?? x.Key?.LocationShort
                        })
                    })
                }
            });
        }

        // GET: api/calendars/{calendarid}/lametric
        [HttpGet("{calendarid}/lametric")]
        public ActionResult LaMetric([FromRoute] string calendarid, [FromQuery] string password)
        {
            const int daysToShow = 4;

            if (password != "9bb04cf87b69e851")
                return StatusCode(403);

            if (!Guid.TryParse(calendarid, out Guid calendarGuid))
                return StatusCode(404);
            
            var days = Enumerable.Range(0, daysToShow * 7 / 5 + 3)
                .Select(i => DateTime.Now.Date.AddDays(i).ToLocalTime())
                .Where(d => d.DayOfWeek >= DayOfWeek.Monday && d.DayOfWeek <= DayOfWeek.Friday)
                .SkipWhile(d => d.Date <= DateTime.Now && DateTime.Now.TimeOfDay.Hours > 18)
                .Take(daysToShow);

            var startTime = days.First().ToUniversalTime();
            var endTime = days.Last().AddDays(1).ToUniversalTime();

            var entries = _dbContext.CalendarOccurances
                .Include(occur => occur.CalendarAppointment)
                .Where(occur => occur.CalendarAppointment.CalendarId == calendarGuid)
                .Where(occur => occur.EndTime >= startTime)
                .Where(occur => occur.StartTime < endTime)
                .ToList();

            var frameData = days.Select(day =>
            {
                var texts = new List<string>();

                var holiday = entries.Where(e => (e.ExBusyState ?? e.CalendarAppointment.BusyState) == 2
                        && e.IsFullDay && e.StartTime.Date <= day && e.EndTime.Date >= day).FirstOrDefault();
                var meetings = entries.Where(e => (e.ExBusyState ?? e.CalendarAppointment.BusyState) == 0
                        && !e.IsFullDay && e.StartTime.Date == day.Date && e.EndTime.Date == day.Date).OrderBy(e => e.StartTime).ToList();

                if (holiday != null)
                    texts.Add($"Urlaub {holiday.CalendarAppointment.Summary.Split(' ', 2).FirstOrDefault() ?? string.Empty}");
                else if (meetings.Any())
                {
                    texts.Add($"S {meetings.First().StartTime.ToLocalTime().ToString("HH':'mm")}");
                    texts.Add(meetings.First().ExLocationShort ?? meetings.First().CalendarAppointment.LocationShort);
                    texts.Add($"E {meetings.Last().EndTime.ToLocalTime().ToString("HH':'mm")}");
                }

                return new KeyValuePair<DateTime, List<string>>(day, texts);
            })
            .SelectMany(e => e.Value.Select(i => new KeyValuePair<DateTime, string>(e.Key, i)))
            .Where(e => !string.IsNullOrWhiteSpace(e.Value));

            return Json(new
            {
                frames = frameData.Select(d =>
                    new
                    {
                        text = d.Value,
                        icon = $"i{d.Key.Day + 11542}"
                    })
            });
        }
    }
}
