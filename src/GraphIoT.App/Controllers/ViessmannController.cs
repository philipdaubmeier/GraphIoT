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

            return Json(res.Data.Select(x => new
            {
                id = x.Id,
                desc = x.Description
            }));
        }

        // GET: api/viessmann/features
        [HttpGet("features")]
        public async Task<JsonResult> GetAllFeatures()
        {
            var features = await _platformClient.GetFeatures();
            
            return Json(new
            {
                features = features.Features.Select(x=>new
                {
                    name = x.Name,
                    properties = x.Properties.GetProperties().Select(p => new
                    {
                        name = p.property,
                        p.value
                    })
                })
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