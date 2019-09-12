using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.NetatmoHost.Polling;
using PhilipDaubmeier.NetatmoHost.Structure;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/netatmo")]
    public class NetatmoController : Controller
    {
        private readonly INetatmoPollingService _pollingService;
        private readonly INetatmoDeviceService _netatmoStructure;

        public NetatmoController(INetatmoPollingService pollingService, INetatmoDeviceService netatmoStructure)
        {
            _pollingService = pollingService;
            _netatmoStructure = netatmoStructure;
        }
        
        // POST api/netatmo/structure/reload
        [HttpPost("structure/reload")]
        public async Task<ActionResult> ReloadStructure()
        {
            _netatmoStructure.ReloadFromNetatmoApi();
            return StatusCode(200);
        }

        // POST api/netatmo/poll
        [HttpPost("poll")]
        public async Task<ActionResult> PollManually([FromQuery] string begin, [FromQuery] string end)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode(404);

            try
            {
                foreach (var day in span.IncludedDates())
                    await _pollingService.PollSensorValues(day, day.AddDays(1));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("rate limit", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = Json(new { ok = false, error = "rate limit reached at netatmo api!" });
                    result.StatusCode = 429; // 429 rate limit reached
                    return result;
                }
                else if (ex.Message.Contains("authenticate", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = Json(new { ok = false, error = "could not authenticate at netatmo api!" });
                    result.StatusCode = 502; // 502 bad gateway, server could not authenticate at remote server
                    return result;
                }
                else
                {
                    var result = Json(new { ok = false, error = "unknown reason" });
                    result.StatusCode = 500;
                    return result;
                }
            }

            return StatusCode(200);
        }
    }
}