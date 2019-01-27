using Microsoft.EntityFrameworkCore;
using Ical.Net.CalendarComponents;
using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Database.Model;
using SmarthomeApi.FormatParsers;
using SmarthomeApi.FormatParsers.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/calendars")]
    public class CalendarController : Controller
    {
        private readonly PersistenceContext _dbContext;
        public CalendarController(PersistenceContext databaseContext)
        {
            _dbContext = databaseContext;
        }

        // POST: api/calendars/{calendarid}/update
        [HttpPost("{calendarid}/update")]
        public async Task<ActionResult> Post()
        {
            var parser = new IcsParser(_dbContext, "philip.daubmeier@audi.de");
            using (var reader = new StreamReader(Request.Body))
            {
                await parser.Parse(reader, 30, 30);
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
                        start = y.Period.StartTime?.Value,
                        end = y.Period.EndTime?.Value,
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

            var startTime = CalendarWeekUtility.FirstDateOfWeek(yearNum, weekNum);
            var endTime = startTime.AddDays(7);

            var calendar = _dbContext.Calendars.Where(cal => cal.Id == calendarGuid).FirstOrDefault();
            var entries = _dbContext.CalendarOccurances
                .Include(occur => occur.CalendarAppointment)
                .Where(occur => occur.CalendarAppointment.CalendarId == calendarGuid)
                .Where(occur => occur.EndTime >= startTime)
                .Where(occur => occur.StartTime < endTime)
                .ToList().GroupBy(x => x.CalendarAppointment)
                .Where(group => group.Count() > 0);
            
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
                        start = startTime,
                        end = endTime
                    },
                    events = entries.Select(x => new
                    {
                        id = x.Key?.Id,
                        summary = x.Key?.Summary,
                        busy = x.Key?.BusyState == 1 ? "tentative" : x.Key?.BusyState == 2 ? "out_of_office" : "busy",
                        @private = x.Key?.IsPrivate,
                        occurences = x.Select(o => new
                        {
                            start = o.StartTime,
                            end = o.EndTime,
                            allday = o.IsFullDay
                        })
                    })
                }
            });
        }
    }
}
