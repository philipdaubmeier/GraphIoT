using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.ViessmannClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/viessmann")]
    public class ViessmannController : Controller
    {
        private readonly ViessmannEstrellaClient _estrellaClient;
        private readonly ViessmannPlatformClient _platformClient;
        private readonly ViessmannVitotrolClient _vitotrolClient;
        
        private readonly PersistenceContext db;
        public ViessmannController(PersistenceContext databaseContext, ViessmannEstrellaClient estrellaClient, ViessmannPlatformClient platformClient, ViessmannVitotrolClient vitotrolClient)
        {
            db = databaseContext;
            _estrellaClient = estrellaClient;
            _platformClient = platformClient;
            _vitotrolClient = vitotrolClient;
        }

        // GET: api/viessmann/installations/names
        [HttpGet("installations/names")]
        public async Task<JsonResult> GetInstallationNames()
        {
            var gatewayIds = await _estrellaClient.GetGateways();
            var controllers = new List<Tuple<int, List<Tuple<int, string>>>>();
            foreach (var gatewayId in gatewayIds)
                controllers.Add(new Tuple<int, List<Tuple<int, string>>>(gatewayId,
                    await _estrellaClient.GetControllers(gatewayId)));

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
            var res = await _platformClient.GetInstallations();

            return Json(Newtonsoft.Json.JsonConvert.DeserializeObject(res));
        }

        // GET: api/viessmann/features
        [HttpGet("features")]
        public async Task<JsonResult> GetAllFeatures()
        {
            var outsideTemp = await _platformClient.GetOutsideTemperature();
            var boilerTemp = await _platformClient.GetBoilerTemperature();
            var burnerActive = await _platformClient.GetBurnerActiveStatus();
            var burnerStats = await _platformClient.GetBurnerStatistics();
            var circuit0Mode = await _platformClient.GetCircuitOperatingMode(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit0Program = await _platformClient.GetCircuitActiveProgram(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit0Normal = await _platformClient.GetCircuitProgramNormal(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit0Reduced = await _platformClient.GetCircuitProgramReduced(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit0Comfort = await _platformClient.GetCircuitProgramComfort(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit0Temp = await _platformClient.GetCircuitTemperature(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit0Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit1Mode = await _platformClient.GetCircuitOperatingMode(ViessmannPlatformClient.Circuit.Circuit1);
            var circuit1Program = await _platformClient.GetCircuitActiveProgram(ViessmannPlatformClient.Circuit.Circuit1);
            var circuit1Normal = await _platformClient.GetCircuitProgramNormal(ViessmannPlatformClient.Circuit.Circuit1);
            var circuit1Reduced = await _platformClient.GetCircuitProgramReduced(ViessmannPlatformClient.Circuit.Circuit1);
            var circuit1Comfort = await _platformClient.GetCircuitProgramComfort(ViessmannPlatformClient.Circuit.Circuit1);
            var circuit1Temp = await _platformClient.GetCircuitTemperature(ViessmannPlatformClient.Circuit.Circuit1);
            var circuit1Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit1);
            var dhwTemp = await _platformClient.GetDhwStorageTemperature();
            var dhwPrimPump = await _platformClient.GetDhwPrimaryPump();
            var dhwCircPump = await _platformClient.GetDhwCirculationPump();
            var boilerTempMain = await _platformClient.GetBoilerTemperatureMain();
            var burnerModulation = await _platformClient.GetBurnerModulation();

            return Json(new
            {
                outside_temp = new { status = outsideTemp.Item1, value = outsideTemp.Item2 },
                boiler_temp = boilerTemp,
                burner_active = burnerActive,
                burner_stats = new { hours = burnerStats.Item1, starts = burnerStats.Item2 },
                circuit_0_mode = circuit0Mode,
                circuit_0_program = circuit0Program,
                circuit_0_normal = new { active = circuit0Normal.Item1, temperature = circuit0Normal.Item2 },
                circuit_0_reduced = new { active = circuit0Reduced.Item1, temperature = circuit0Reduced.Item2 },
                circuit_0_comfort = new { active = circuit0Comfort.Item1, temperature = circuit0Comfort.Item2 },
                circuit_0_temp = new { status = circuit0Temp.Item1, value = circuit0Temp.Item2 },
                circuit_0_pump = circuit0Pump,
                circuit_1_mode = circuit1Mode,
                circuit_1_program = circuit1Program,
                circuit_1_normal = new { active = circuit1Normal.Item1, temperature = circuit1Normal.Item2 },
                circuit_1_reduced = new { active = circuit1Reduced.Item1, temperature = circuit1Reduced.Item2 },
                circuit_1_comfort = new { active = circuit1Comfort.Item1, temperature = circuit1Comfort.Item2 },
                circuit_1_temp = new { status = circuit1Temp.Item1, value = circuit1Temp.Item2 },
                circuit_1_pump = circuit1Pump,
                dhw_temp = new { status = dhwTemp.Item1, value = dhwTemp.Item2 },
                dhw_prim_pump = dhwPrimPump,
                dhw_circ_pump = dhwCircPump,
                boiler_temp_main = new { status = boilerTempMain.Item1, value = boilerTempMain.Item2 },
                burner_modulation = burnerModulation
            });
        }

        // GET: api/viessmann/features/selected
        [HttpGet("features/selected")]
        public async Task<JsonResult> GetSelectedFeatures()
        {
            var burnerStatistics = await _platformClient.GetBurnerStatistics();
            var burnerHoursTotal = burnerStatistics.Item1;
            var burnerStartsTotal = burnerStatistics.Item2;
            var burnerModulation = await _platformClient.GetBurnerModulation();

            var outsideTemp = (await _platformClient.GetOutsideTemperature()).Item2;
            var boilerTemp = await _platformClient.GetBoilerTemperature();
            var boilerTempMain = (await _platformClient.GetBoilerTemperatureMain()).Item2;
            var circuit0Temp = (await _platformClient.GetCircuitTemperature(ViessmannPlatformClient.Circuit.Circuit0)).Item2;
            var circuit1Temp = (await _platformClient.GetCircuitTemperature(ViessmannPlatformClient.Circuit.Circuit1)).Item2;
            var dhwTemp = (await _platformClient.GetDhwStorageTemperature()).Item2;

            var burnerActive = await _platformClient.GetBurnerActiveStatus();
            var circuit0Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit1Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit1);
            var dhwPrimPump = await _platformClient.GetDhwPrimaryPump();
            var dhwCircPump = await _platformClient.GetDhwCirculationPump();

            return Json(new
            {
                burner_hours_total = burnerHoursTotal,
                burner_starts_total = burnerStartsTotal,
                burner_modulation = burnerModulation,
                outside_temp = outsideTemp,
                boiler_temp = boilerTemp,
                boiler_temp_main = boilerTempMain,
                circuit_0_temp = circuit0Temp,
                circuit_1_temp = circuit1Temp,
                dhw_temp = dhwTemp,
                burner_active = burnerActive,
                circuit_0_pump = circuit0Pump,
                circuit_1_pump = circuit1Pump,
                dhw_prim_pump = dhwPrimPump,
                dhw_circ_pump = dhwCircPump,
            });
        }

        // GET: api/viessmann/pumps
        [HttpGet("pumps")]
        public async Task<JsonResult> GetPumpStates()
        {
            var burnerActive = await _platformClient.GetBurnerActiveStatus();
            var circuit0Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit0);
            var circuit1Pump = await _platformClient.GetCircuitCirculationPump(ViessmannPlatformClient.Circuit.Circuit1);
            var dhwPrimPump = await _platformClient.GetDhwPrimaryPump();
            var dhwCircPump = await _platformClient.GetDhwCirculationPump();
            var solarCircPump = (await _vitotrolClient.GetData(new List<int>() { 5274 })).FirstOrDefault().Item2?.Trim() != "0";

            return Json(new
            {
                burner_active = burnerActive,
                circuit_0_pump = circuit0Pump,
                circuit_1_pump = circuit1Pump,
                dhw_prim_pump = dhwPrimPump,
                dhw_circ_pump = dhwCircPump,
                solar_circ_pump = solarCircPump
            });
        }

        // GET: api/viessmann/datapoints
        [HttpGet("datapoints")]
        public async Task<JsonResult> GetDatapoints()
        {
            var datapoints = await _vitotrolClient.GetTypeInfo();

            return Json(new
            {
                datapoints = datapoints.Select(x => new
                {
                    id = x.Key,
                    name = x.Value
                })
            });
        }

        // GET: api/viessmann/heating/curves/days/{day}
        [HttpGet("heating/curves/days/{day}")]
        public ActionResult GetHeatingCurves([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode(404);

            var dbHeatingSeries = db.ViessmannHeatingTimeseries.Where(x => x.Day == dayDate.Date).FirstOrDefault();
            if (dbHeatingSeries == null)
                return StatusCode(404);

            return Json(new
            {
                burner_hours_total = dbHeatingSeries.BurnerHoursTotal,
                burner_starts_total = dbHeatingSeries.BurnerStartsTotal,
                burner_minutes = dbHeatingSeries.BurnerMinutesSeries.Trimmed(0d),
                burner_starts = dbHeatingSeries.BurnerStartsSeries.Trimmed(0),
                burner_modulation = dbHeatingSeries.BurnerModulationSeries.Trimmed(0),
                outside_temp = dbHeatingSeries.OutsideTempSeries.Trimmed(0d),
                boiler_temp = dbHeatingSeries.BoilerTempSeries.Trimmed(0d),
                boiler_temp_main = dbHeatingSeries.BoilerTempMainSeries.Trimmed(0d),
                circuit_0_temp = dbHeatingSeries.Circuit0TempSeries.Trimmed(0d),
                circuit_1_temp = dbHeatingSeries.Circuit1TempSeries.Trimmed(0d),
                dhw_temp = dbHeatingSeries.DhwTempSeries.Trimmed(0d),
                burner_active = dbHeatingSeries.BurnerActiveSeries.Trimmed(false),
                circuit_0_pump = dbHeatingSeries.Circuit0PumpSeries.Trimmed(false),
                circuit_1_pump = dbHeatingSeries.Circuit1PumpSeries.Trimmed(false),
                dhw_prim_pump = dbHeatingSeries.DhwPrimaryPumpSeries.Trimmed(false),
                dhw_circ_pump = dbHeatingSeries.DhwCirculationPumpSeries.Trimmed(false),
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
            var chartSolarYield = dbSolarSeries.SolarWhSeries.Trimmed(0).TakeLast(37);
            var currentCollectorTemp = dbSolarSeries.SolarCollectorTempSeries.Reverse()
                .SkipWhile(x => !x.Value.HasValue).FirstOrDefault().Value;

            return Json(new
            {
                frames = new List<object>() {
                    new
                    {
                        text = $"{totalYieldToday:0.#} kWh",
                        icon = "a30793"
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