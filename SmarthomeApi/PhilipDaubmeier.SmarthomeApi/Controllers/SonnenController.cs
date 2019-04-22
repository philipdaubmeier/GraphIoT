using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.SmarthomeApi.Clients.Sonnen;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/sonnen")]
    public class SonnenController : Controller
    {
        private static SonnenPortalClient _sonnenClient;

        public SonnenController(SonnenPortalClient sonnenClient)
        {
            _sonnenClient = sonnenClient;
        }

        // GET api/sonnen/history/week
        [HttpGet("history/week")]
        public async Task<JsonResult> GetLastWeekValues()
        {
            var values = await _sonnenClient.GetEnergyStats(DateTime.Now.AddDays(-7));

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