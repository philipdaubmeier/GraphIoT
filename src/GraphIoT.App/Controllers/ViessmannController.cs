using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.ViessmannClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Produces("application/json")]
    [Route("api/viessmann")]
    public class ViessmannController : Controller
    {
        private readonly ViessmannEstrellaClient _estrellaClient;
        private readonly ViessmannPlatformClient _platformClient;
        private readonly ViessmannVitotrolClient _vitotrolClient;

        public ViessmannController(ViessmannEstrellaClient estrellaClient, ViessmannPlatformClient platformClient, ViessmannVitotrolClient vitotrolClient)
        {
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
    }
}