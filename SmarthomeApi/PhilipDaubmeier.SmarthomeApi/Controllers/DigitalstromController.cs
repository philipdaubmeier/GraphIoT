using Microsoft.AspNetCore.Mvc;
using NodaTime;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromHost.Polling;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/digitalstrom")]
    public class DigitalstromController : Controller
    {
        private static DigitalstromDssClient dsClient;

        private readonly PersistenceContext db;
        private readonly DigitalstromEnergyPollingService _pollingService;

        public DigitalstromController(PersistenceContext databaseContext, IDigitalstromConnectionProvider connectionProvider, IEnumerable<IDigitalstromPollingService> pollingServices)
        {
            db = databaseContext;
            dsClient = new DigitalstromDssClient(connectionProvider);
            _pollingService = pollingServices.Select(x => x as DigitalstromEnergyPollingService).Where(x => x != null).FirstOrDefault();
        }

        // GET api/digitalstrom/sensors
        [HttpGet("sensors")]
        public async Task<JsonResult> GetSensors()
        {
            var sensorValues = (await dsClient.GetZonesAndSensorValues()).Zones;

            return Json(new
            {
                zones = sensorValues.Select(x => new Tuple<int, List<double>, List<double>>(
                        x.ZoneID,
                        x?.Sensor?.Where(s => s.Type == 9).Select(s => s.Value).ToList(),
                        x?.Sensor?.Where(s => s.Type == 13).Select(s => s.Value).ToList()
                    ))
                    .Where(x => (x.Item2?.Count ?? 0) > 0 || (x.Item3?.Count ?? 0) > 0)
                    .Select(x => new
                    {
                        zone_id = x.Item1,
                        temperature = (x.Item2?.Count ?? 0) > 0 ? (double?)x.Item2.First() : null,
                        humidity = (x.Item3?.Count ?? 0) > 0 ? (double?)x.Item3.First() : null
                    })
            });
        }

        // GET: api/digitalstrom/sensors/curves/days/{day}
        [HttpGet("sensors/curves/days/{day}")]
        public ActionResult GetSensorsCurves([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode(404);

            var sensorData = db.DsSensorDataSet.Where(x => x.Day == dayDate.Date).ToList();
            if (sensorData == null)
                return StatusCode(404);

            return Json(new
            {
                zones = sensorData.Select(zone => new
                {
                    id = zone.ZoneId,
                    temperature_curve = zone.TemperatureSeries.Trimmed(0d),
                    humidity_curve = zone.HumiditySeries.Trimmed(0d)
                })
            });
        }

        // GET: api/digitalstrom/energy/curves/days/{day}
        [HttpGet("energy/curves/days/{day}")]
        public async Task<ActionResult> GetEnergyCurves([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode(404);
            
            var energyData = db.DsEnergyHighresDataSet.Where(x => x.Day == dayDate.Date).FirstOrDefault();
            if (energyData == null)
                return StatusCode(404);

            var circuitNames = (await dsClient.GetMeteringCircuits()).FilteredMeterNames;

            DateTime getBegin(ITimeSeries<int> ts) => ts.SkipWhile(t => !t.Value.HasValue).FirstOrDefault().Key;

            return Json(new
            {
                circuits = energyData.EnergySeriesEveryMeter.Select(circuit => new
                {
                    dsuid = circuit.Key.ToString(),
                    name = circuitNames.ContainsKey(circuit.Key) ? circuitNames[circuit.Key] : null,
                    begin = Instant.FromDateTimeUtc(getBegin(circuit.Value).ToUniversalTime()).ToUnixTimeSeconds(),
                    energy_curve = circuit.Value.Trimmed(-1)
                })
            });
        }

        // GET: api/digitalstrom/events/callscene/days/{day}
        [HttpGet("events/callscene/days/{day}")]
        public ActionResult GetCallSceneEvents([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode(404);

            var eventData = db.DsSceneEventDataSet.Where(x => x.Day == dayDate.Date).FirstOrDefault();
            if (eventData == null)
                return StatusCode(404);
            
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

        // POST api/digitalstrom/energy/midlowres/compute
        [HttpPost("energy/midlowres/compute")]
        public ActionResult ComputeMidLowResFromHighRes([FromQuery] string begin, [FromQuery] string end)
        {
            if (_pollingService == null)
                return StatusCode(400);

            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode(404);

            _pollingService.GenerateMidLowResEnergySeries(span.Begin, span.End);

            return StatusCode(200);
        }
    }
}