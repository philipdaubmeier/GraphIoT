using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Database.Model;
using SmarthomeApi.FormatParsers;
using System.IO;
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
        public async Task<ActionResult> Post([FromBody]string value)
        {
            var parser = new IcsParser(_dbContext, "philip.daubmeier@audi.de");
            using (var reader = new StreamReader(Request.Body))
            {
                await parser.Parse(reader);
            }

            return StatusCode(200);
        }
    }
}
