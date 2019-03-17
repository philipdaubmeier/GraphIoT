using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmarthomeApi.Clients.Digitalstrom;
using SmarthomeApi.Database.Model;

namespace SmarthomeApi.Controllers
{
    [Route("powerconsumption")]
    public class PowerConsumptionController : Controller
    {
        private static DigitalstromClient dsClient;

        private readonly PersistenceContext db;
        public PowerConsumptionController(PersistenceContext databaseContext)
        {
            db = databaseContext;
            dsClient = new DigitalstromClient(db);
        }

        // GET powerconsumption
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var powerValues = (await dsClient.GetTotalEnergy(60, 37))
                .TakeLast(37).Select(x => (int)Math.Round(x.Value, 0)).ToList();

            return Json(new
            { frames = new List<object>() {
                    new
                    {
                        text = $"{powerValues.LastOrDefault()} W",
                        icon = "a21256"
                    },
                    new
                    {
                        index = 1,
                        chartData = powerValues
                    }
                }
            });
        }
    }
}
