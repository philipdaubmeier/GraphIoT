using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhilipDaubmeier.SmarthomeApi.Clients;
using PhilipDaubmeier.SmarthomeApi.Clients.Sonnen;
using PhilipDaubmeier.SmarthomeApi.Database.Model;
using PhilipDaubmeier.SmarthomeApi.Model.Config;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/sonnen")]
    public class SonnenController : Controller
    {
        private static SonnenPortalClient sonnenClient;

        private readonly PersistenceContext db;
        public SonnenController(TokenStore<SonnenPortalClient> tokenStore, PersistenceContext databaseContext, IOptions<SonnenConfig> config)
        {
            db = databaseContext;
            sonnenClient = new SonnenPortalClient(tokenStore, config);
        }

        // GET api/sonnen/history/week
        [HttpGet("history/week")]
        public async Task<JsonResult> GetLastWeekValues()
        {
            var values = await sonnenClient.GetEnergyStats(DateTime.Now.AddDays(-7));

            return Json(new
            {
                values = values.Select(x => new
                {
                    time = x.Key,
                    discharge = x.Value.Discharge,
                    charge = x.Value.Charge,
                    pvproduction = x.Value.PvProduction,
                    consumption = x.Value.Consumption,
                    soc = x.Value.SOC
                })
            });
        }
    }
}