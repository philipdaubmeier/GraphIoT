using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Polling;
using PhilipDaubmeier.GraphIoT.Netatmo.Polling;
using PhilipDaubmeier.GraphIoT.Sonnen.Polling;
using PhilipDaubmeier.GraphIoT.Viessmann.Polling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Route("api/triggerpolling")]
    public class TriggerPollingController : Controller
    {
        private readonly DigitalstromEnergyPollingService _energyPollingService;
        private readonly DigitalstromSensorPollingService _sensorPollingService;
        private readonly ViessmannHeatingPollingService _heatingPollingService;
        private readonly ViessmannSolarPollingService _solarPollingService;
        private readonly ISonnenPollingService _sonnenPollingService;
        private readonly INetatmoPollingService _netatmoPollingService;

        public TriggerPollingController(IEnumerable<IDigitalstromPollingService> dsPollingServices, IEnumerable<IViessmannPollingService> viessPollingServices, ISonnenPollingService sonnenPollingService, INetatmoPollingService netatmoPollingService)
        {
            _energyPollingService = dsPollingServices.Select(x => x as DigitalstromEnergyPollingService).Where(x => x != null).First()!;
            _sensorPollingService = dsPollingServices.Select(x => x as DigitalstromSensorPollingService).Where(x => x != null).First()!;
            _heatingPollingService = viessPollingServices.Select(x => x as ViessmannHeatingPollingService).Where(x => x != null).First()!;
            _solarPollingService = viessPollingServices.Select(x => x as ViessmannSolarPollingService).Where(x => x != null).First()!;
            _sonnenPollingService = sonnenPollingService;
            _netatmoPollingService = netatmoPollingService;
        }

        // POST api/triggerpolling/digitalstrom/energy/midlowres/compute
        [HttpPost("digitalstrom/energy/midlowres/compute")]
        public ActionResult ComputeDsEnergyMidLowResFromHighRes([FromQuery] string begin, [FromQuery] string end)
        {
            if (_energyPollingService == null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

            _energyPollingService.GenerateMidLowResEnergySeries(span.Begin, span.End);

            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/triggerpolling/digitalstrom/sensors/lowres/compute
        [HttpPost("digitalstrom/sensors/lowres/compute")]
        public ActionResult ComputeDsSensorLowResFromMidRes([FromQuery] string begin, [FromQuery] string end)
        {
            if (_sensorPollingService == null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

            _sensorPollingService.GenerateLowResSensorSeries(span.Begin, span.End);

            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/triggerpolling/viessmann/solar/lowres/compute
        [HttpPost("viessmann/solar/lowres/compute")]
        public ActionResult ComputeSolarLowResFromMidRes([FromQuery] string begin, [FromQuery] string end)
        {
            if (_solarPollingService == null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

            _solarPollingService.GenerateLowResSolarSeries(span.Begin, span.End);

            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/triggerpolling/viessmann/heating/lowres/compute
        [HttpPost("viessmann/heating/lowres/compute")]
        public ActionResult ComputeHeatingLowResFromMidRes([FromQuery] string begin, [FromQuery] string end)
        {
            if (_heatingPollingService == null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

            _heatingPollingService.GenerateLowResHeatingSeries(span.Begin, span.End);

            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/triggerpolling/sonnen/poll
        [HttpPost("sonnen/poll")]
        public async Task<ActionResult> PollSonnenManually([FromQuery] string begin, [FromQuery] string end)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

            foreach (var day in span.IncludedDates())
                await _sonnenPollingService.PollSensorValues(day, day.AddDays(1));

            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/triggerpolling/netatmo/poll
        [HttpPost("netatmo/poll")]
        public async Task<ActionResult> PollNetatmoManually([FromQuery] string begin, [FromQuery] string end)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

            ActionResult Error(int statusCode, string message)
            {
                var result = Json(new { ok = false, error = message });
                result.StatusCode = statusCode;
                return result;
            }

            try
            {
                foreach (var day in span.IncludedDates())
                    await _netatmoPollingService.PollSensorValues(day, day.AddDays(1));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("rate limit", StringComparison.InvariantCultureIgnoreCase))
                    return Error(429, "rate limit reached at netatmo api!");
                else if (ex.Message.Contains("authenticate", StringComparison.InvariantCultureIgnoreCase))
                    return Error(502, "could not authenticate at netatmo api!");
                else
                    return Error(500, "unknown reason");
            }

            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}