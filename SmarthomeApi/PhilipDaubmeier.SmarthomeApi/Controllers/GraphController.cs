using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.DigitalstromHost.ViewModel;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;
using PhilipDaubmeier.ViessmannHost.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/graph")]
    public class GraphController : Controller
    {
        private readonly PersistenceContext db;
        private readonly DigitalstromDbContext dsDb;
        public GraphController(PersistenceContext databaseContext, DigitalstromDbContext dsDatabaseContext)
        {
            db = databaseContext;
            dsDb = dsDatabaseContext;
        }

        private JsonResult GetGraphs(IEnumerable<GraphViewModel> graphs)
        {
            return Json(new
            {
                lines = graphs.Select(g => new
                {
                    begin = g.BeginUnixTimestamp,
                    spacing = g.SpacingMillis,
                    name = g.Name,
                    format = g.Format,
                    points = g.Points
                })
            });
        }

        // GET: api/graph/solar
        [HttpGet("solar")]
        public ActionResult GetSolarGraph([FromQuery] string begin, [FromQuery] string end, [FromQuery] string count)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, count, out TimeSeriesSpan span))
                return StatusCode(404);

            var graphs = new ViessmannSolarViewModel(db, span);
            if (graphs.IsEmpty)
                return StatusCode(404);

            return GetGraphs(graphs.Graphs());
        }

        // GET: api/graph/heating
        [HttpGet("heating")]
        public ActionResult GetHeatingGraph([FromQuery] string begin, [FromQuery] string end, [FromQuery] string count)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, count, out TimeSeriesSpan span))
                return StatusCode(404);

            var graphs = new ViessmannHeatingViewModel(db, span);
            if (graphs.IsEmpty)
                return StatusCode(404);

            return GetGraphs(graphs.Graphs());
        }

        // GET: api/graph/sensors
        [HttpGet("sensors")]
        public ActionResult GetSensorsGraph([FromQuery] string begin, [FromQuery] string end, [FromQuery] string count)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, count, out TimeSeriesSpan span))
                return StatusCode(404);

            var graphs = new DigitalstromZoneSensorViewModel(dsDb, span);
            if (graphs.IsEmpty)
                return StatusCode(404);

            return GetGraphs(graphs.Graphs());
        }

        // GET: api/graph/energy
        [HttpGet("energy")]
        public ActionResult GetEnergyGraph([FromQuery] string begin, [FromQuery] string end, [FromQuery] string count)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, count, out TimeSeriesSpan span))
                return StatusCode(404);

            var graphs = new DigitalstromEnergyViewModel(dsDb, span);
            if (graphs.IsEmpty)
                return StatusCode(404);

            return GetGraphs(graphs.Graphs());
        }
    }
}