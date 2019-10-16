using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.ViessmannClient;
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
            var controllers = new List<(int gatewayId, List<(int controllerId, string typeName)> constrollers)>();
            foreach (var gatewayId in gatewayIds)
                controllers.Add((gatewayId, await _estrellaClient.GetControllers(gatewayId)));

            return Json(new
            {
                gateways = controllers.Select(g => new
                {
                    id = g.gatewayId,
                    controllers = g.constrollers.Select(c => new
                    {
                        id = c.controllerId,
                        name = c.typeName
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
                outside_temp = new { outsideTemp.status, value = outsideTemp.temperature },
                boiler_temp = boilerTemp,
                burner_active = burnerActive,
                burner_stats = new { burnerStats.hours, burnerStats.starts },
                circuit_0_mode = circuit0Mode,
                circuit_0_program = circuit0Program,
                circuit_0_normal = new { circuit0Normal.active, circuit0Normal.temperature },
                circuit_0_reduced = new { circuit0Reduced.active, circuit0Reduced.temperature },
                circuit_0_comfort = new { circuit0Comfort.active, circuit0Comfort.temperature },
                circuit_0_temp = new { circuit0Temp.status, value = circuit0Temp.temperature },
                circuit_0_pump = circuit0Pump,
                circuit_1_mode = circuit1Mode,
                circuit_1_program = circuit1Program,
                circuit_1_normal = new { circuit1Normal.active, circuit1Normal.temperature },
                circuit_1_reduced = new { circuit1Reduced.active, circuit1Reduced.temperature },
                circuit_1_comfort = new { circuit1Comfort.active, circuit1Comfort.temperature },
                circuit_1_temp = new { circuit1Temp.status, value = circuit1Temp.temperature },
                circuit_1_pump = circuit1Pump,
                dhw_temp = new { dhwTemp.status, value = dhwTemp.temperature },
                dhw_prim_pump = dhwPrimPump,
                dhw_circ_pump = dhwCircPump,
                boiler_temp_main = new { boilerTempMain.status, value = boilerTempMain.temperature },
                burner_modulation = burnerModulation
            });
        }

        // GET: api/viessmann/datapoints
        [HttpGet("datapoints")]
        public async Task<JsonResult> GetDatapoints()
        {
            var datapoints = await _vitotrolClient.GetTypeInfo();

            return Json(new { datapoints = datapoints.Select(x => new { x.id, x.name }) });
        }
    }
}