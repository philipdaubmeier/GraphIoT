using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Clients.Viessmann;
using SmarthomeApi.Database.Model;

namespace SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/viessmann")]
    public class ViessmannController : Controller
    {
        private readonly ViessmannEstrellaClient estrellaClient;
        private readonly ViessmannPlatformClient platformClient;
        private readonly ViessmannVitotrolClient vitotrolClient;

        private readonly PersistenceContext db;
        public ViessmannController(PersistenceContext databaseContext)
        {
            db = databaseContext;
            estrellaClient = new ViessmannEstrellaClient();
            platformClient = new ViessmannPlatformClient(db);
            vitotrolClient = new ViessmannVitotrolClient(db);
        }

        // GET: api/viessmann/installations/names
        [HttpGet("installations/names")]
        public async Task<JsonResult> GetInstallationNames()
        {
            var gatewayIds = await estrellaClient.GetGateways();
            var controllers = new List<Tuple<int, List<Tuple<int, string>>>>();
            foreach (var gatewayId in gatewayIds)
                controllers.Add(new Tuple<int, List<Tuple<int, string>>>(gatewayId,
                    await estrellaClient.GetControllers(gatewayId)));

            return Json(new
            {
                gateways = controllers.Select(g => new
                {
                    id = g.Item1,
                    controllers = g.Item2.Select(c => new
                    {
                        id = c.Item1,
                        name = c.Item2
                    })
                })
            });
        }

        // GET: api/viessmann/installations
        [HttpGet("installations")]
        public async Task<JsonResult> GetInstallations()
        {
            var res = await platformClient.GetInstallations();

            return Json(new
            {
                result = res
            });
        }

        // GET: api/viessmann/datapoints
        [HttpGet("datapoints")]
        public async Task<JsonResult> GetDatapoints()
        {
            var datapoints = await vitotrolClient.GetTypeInfo();

            return Json(new
            {
                datapoints = datapoints.Select(x => new
                {
                    id = x.Key,
                    name = x.Value
                })
            });
        }

        // GET: api/viessmann/solar/lametric
        [HttpGet("solar/lametric")]
        public ActionResult GetSolarLaMetric()
        {
            var dbSolarSeries = db.ViessmannSolarTimeseries.Where(x => x.Day == DateTime.Now.Date).FirstOrDefault();
            if (dbSolarSeries == null)
                return StatusCode(404);

            var totalYieldToday = (double)dbSolarSeries.SolarWhTotal / 1000d;
            var chartSolarYield = dbSolarSeries.SolarWhSeries.SkipWhile(x => !x.Value.HasValue)
                .Reverse().SkipWhile(x => !x.Value.HasValue).Reverse()
                .Select(x => x.Value ?? 0).TakeLast(37).ToList();
            var currentCollectorTemp = dbSolarSeries.SolarCollectorTempSeries.Reverse()
                .SkipWhile(x => !x.Value.HasValue).FirstOrDefault().Value;

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
                    },
                    new
                    {
                        text = $"{currentCollectorTemp:0.#} °C",
                        icon = "a27285"
                    }
                }
            });
        }

        // GET: api/viessmann/solar/curves/days/{day}
        [HttpGet("solar/curves/days/{day}")]
        public ActionResult GetSolarCurves([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode(404);
            
            var dbSolarSeries = db.ViessmannSolarTimeseries.Where(x => x.Day == dayDate.Date).FirstOrDefault();
            if (dbSolarSeries == null)
                return StatusCode(404);

            var l1 = dbSolarSeries.SolarWhSeries.ToList();
            var l2 = dbSolarSeries.SolarCollectorTempSeries.ToList();
            var l3 = dbSolarSeries.SolarHotwaterTempSeries.ToList();
            var l4 = dbSolarSeries.SolarPumpStateSeries.ToList();
            var l5 = dbSolarSeries.SolarSuppressionSeries.ToList();
            var zippedValues = new List<Tuple<DateTime, int?, double?, double?, bool?, bool?>>();
            for (int i = 0; i < l1.Count; i++)
                zippedValues.Add(new Tuple<DateTime, int?, double?, double?, bool?, bool?>(
                    l1[i].Key, l1[i].Value, l2[i].Value, l3[i].Value, l4[i].Value, l5[i].Value));

            Func<Tuple<DateTime, int?, double?, double?, bool?, bool?>, bool> hasValue = 
                t => t.Item2.HasValue || t.Item3.HasValue || t.Item4.HasValue || t.Item5.HasValue || t.Item6.HasValue;
            var zippedTrimmed = zippedValues.SkipWhile(t => !hasValue(t)).Reverse().SkipWhile(t => !hasValue(t)).Reverse().ToList();

            return Json(new
            {
                begin = zippedTrimmed.FirstOrDefault()?.Item1 ?? l1.FirstOrDefault().Key,
                solardata = zippedTrimmed.Select(x => new
                {
                    wh = x.Item2,
                    collector_temp = x.Item3,
                    dhw_temp = x.Item4,
                    pump_state = x.Item5,
                    suppression = x.Item6
                })
            });
        }
    }
}
