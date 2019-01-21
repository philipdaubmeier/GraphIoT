using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Clients;
using SmarthomeApi.Database.Model;

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
