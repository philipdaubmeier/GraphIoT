using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Polling;
using PhilipDaubmeier.GraphIoT.Netatmo.Structure;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Route("api/netatmo")]
    public class NetatmoController : Controller
    {
        private readonly INetatmoDbContext _databaseContext;
        private readonly INetatmoPollingService _pollingService;
        private readonly INetatmoDeviceService _netatmoStructure;

        public NetatmoController(INetatmoDbContext databaseContext, INetatmoPollingService pollingService, INetatmoDeviceService netatmoStructure)
        {
            _databaseContext = databaseContext;
            _pollingService = pollingService;
            _netatmoStructure = netatmoStructure;
        }

        // DELETE api/netatmo/structure/modules/{moduleId}
        [HttpDelete("structure/modules/{moduleId}")]
        public ActionResult DeleteModule([FromRoute] string moduleId)
        {
            var normalizedModuleId = (string)(ModuleId)moduleId;
            _databaseContext.NetatmoModuleMeasures.RemoveRange(_databaseContext.NetatmoModuleMeasures.Where(x => x.ModuleId == normalizedModuleId));
            _databaseContext.SaveChanges();
            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/netatmo/structure/reload
        [HttpPost("structure/reload")]
        public ActionResult ReloadStructure()
        {
            _netatmoStructure.ReloadFromNetatmoApi();
            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/netatmo/poll
        [HttpPost("poll")]
        public async Task<ActionResult> PollManually([FromQuery] string begin, [FromQuery] string end)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

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

            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}