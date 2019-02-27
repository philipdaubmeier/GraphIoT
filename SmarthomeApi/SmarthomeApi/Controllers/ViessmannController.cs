using System;
using System.Collections.Generic;
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
    }
}
