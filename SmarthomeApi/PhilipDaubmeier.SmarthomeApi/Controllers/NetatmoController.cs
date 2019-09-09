using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.NetatmoHost.Polling;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/netatmo")]
    public class NetatmoController : Controller
    {
        private readonly INetatmoPollingService _pollingService;

        public NetatmoController(INetatmoPollingService pollingService)
        {
            _pollingService = pollingService;
        }

        // POST api/netatmo/poll
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