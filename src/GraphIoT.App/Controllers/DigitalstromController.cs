using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.GraphIoT.App.Database;
using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Route("api/digitalstrom")]
    public class DigitalstromController : Controller
    {
        private readonly PersistenceContext db;

        public DigitalstromController(PersistenceContext databaseContext)
        {
            db = databaseContext;
        }

        // GET: api/digitalstrom/events/callscene/days/{day}
        [HttpGet("events/callscene/days/{day}")]
        public ActionResult GetCallSceneEvents([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode((int)HttpStatusCode.NotFound);

            var eventData = db.DsSceneEventDataSet.Where(x => x.Key == dayDate.Date).FirstOrDefault();
            if (eventData == null)
                return StatusCode((int)HttpStatusCode.NotFound);
            
            return Json(new
            {
                events = eventData.EventStream.Select(dssEvent => new
                {
                    timestamp = dssEvent.TimestampUtc,
                    dssEvent.SystemEvent.Name,
                    zone = (int)dssEvent.Properties.ZoneID,
                    group = (int)dssEvent.Properties.GroupID,
                    scene = (int)dssEvent.Properties.SceneID
                })
            });
        }
    }
}