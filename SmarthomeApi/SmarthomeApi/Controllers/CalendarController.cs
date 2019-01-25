using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Database.Model;
using SmarthomeApi.FormatParsers;
using SmarthomeApi.FormatParsers.Utility;
using System;
using System.Globalization;
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
                await parser.Parse(reader);
            }

            return StatusCode(200);
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

            var calendarQuery = _dbContext.Calendars.Where(cal => cal.Id == calendarGuid);
            var calendar = calendarQuery.FirstOrDefault();
            var entries = calendarQuery.SelectMany(cal => cal.Entries)
                .Where(entry => entry.EndTime >= startTime && entry.StartTime < endTime)
                .ToList();

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
                        id = x.Id,
                        summary = x.Summary,
                        busy = x.BusyState == 1 ? "tentative" : x.BusyState == 2 ? "out_of_office" : "busy",
                        @private = x.IsPrivate,
                        fullday = x.IsFullDay,
                        starttime = x.StartTime,
                        endtime = x.EndTime,
                        rrule = x.RecurranceRule,
                        modified = x.Modified,
                    })
                }
            });
        }
    }
}
