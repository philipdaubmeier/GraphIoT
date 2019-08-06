using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.SonnenClient;
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

        // GET api/sonnen/mysonnentest
        [HttpGet("mysonnentest")]
        public async Task<JsonResult> MySonnenTest()
        {
            var userSites = await _sonnenClient.GetUserSites();
            var siteId = userSites.SiteIds.FirstOrDefault();
            var values = await _sonnenClient.GetEnergyMeasurements(siteId, DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
            var battery = await _sonnenClient.GetBatterySystems(siteId);
            var stats = await _sonnenClient.GetStatistics(siteId, DateTime.Now.Date, DateTime.Now.Date.AddDays(1));

            return Json(new
            {
                vals = values.ConsumptionPower
            });
        }
    }
}