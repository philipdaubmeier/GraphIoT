using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenHost.Polling;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/sonnen")]
    public class SonnenController : Controller
    {
        private static SonnenPortalClient _sonnenClient;
        private static ISonnenPollingService _pollingService;

        public SonnenController(SonnenPortalClient sonnenClient, ISonnenPollingService pollingService)
        {
            _sonnenClient = sonnenClient;
            _pollingService = pollingService;
        }

        // GET api/sonnen/mysonnentest
        [HttpGet("mysonnentest")]
        public async Task<JsonResult> MySonnenTest()
        {
            var userSites = await _sonnenClient.GetUserSites();
            var siteId = userSites.DefaultSiteId;
            var values = await _sonnenClient.GetEnergyMeasurements(siteId, DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
            var battery = await _sonnenClient.GetBatterySystems(siteId);
            var stats = await _sonnenClient.GetStatistics(siteId, DateTime.Now.Date, DateTime.Now.Date.AddDays(1));

            return Json(new
            {
                vals = values.ConsumptionPower
            });
        }

        // POST api/sonnen/poll
        [HttpPost("poll")]
        public async Task<ActionResult> PollManually([FromQuery] string begin, [FromQuery] string end)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode(404);

            foreach (var day in span.IncludedDates())
                await _pollingService.PollSensorValues(day, day.AddDays(1));

            return StatusCode(200);
        }
    }
}