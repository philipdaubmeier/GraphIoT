using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenHost.Database;
using PhilipDaubmeier.SonnenHost.Polling;
using PhilipDaubmeier.SonnenHost.ViewModel;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/sonnen")]
    public class SonnenController : Controller
    {
        private readonly ISonnenDbContext _dbContext;
        private readonly SonnenPortalClient _sonnenClient;
        private readonly ISonnenPollingService _pollingService;

        public SonnenController(ISonnenDbContext databaseContext, SonnenPortalClient sonnenClient, ISonnenPollingService pollingService)
        {
            _dbContext = databaseContext;
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
                return StatusCode((int)HttpStatusCode.NotFound);

            foreach (var day in span.IncludedDates())
                await _pollingService.PollSensorValues(day, day.AddDays(1));

            return StatusCode((int)HttpStatusCode.OK);
        }

        // GET: api/sonnen/lametric
        [HttpGet("lametric")]
        public ActionResult GetSonnenLaMetric()
        {
            var viewModel = new SonnenEnergyViewModel(_dbContext)
            {
                Span = new TimeSeriesSpan(DateTime.Now.Date, DateTime.Now, TimeSeriesSpan.Spacing.Spacing1Min)
            };
            var totalYieldToday = viewModel.Graph(0).Points.Sum(x => (decimal)x) / 60 / 1000;

            viewModel.Span = new TimeSeriesSpan(DateTime.Now.Date, DateTime.Now.Date.AddDays(1), 37);
            var chartSolarYield = viewModel.Graph(0).Points;

            return Json(new
            {
                frames = new List<object>() {
                    new
                    {
                        text = $"{totalYieldToday:0.#} kWh",
                        icon = "a27283"
                    },
                    new
                    {
                        index = 2,
                        chartData = chartSolarYield
                    }
                }
            });
        }
    }
}