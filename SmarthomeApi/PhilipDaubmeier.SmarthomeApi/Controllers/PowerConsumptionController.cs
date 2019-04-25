using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Network;
using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.SmarthomeApi.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("powerconsumption")]
    public class PowerConsumptionController : Controller
    {
        private static DigitalstromWebserviceClient dsClient;

        private readonly PersistenceContext db;
        public PowerConsumptionController(PersistenceContext databaseContext, IDigitalstromConnectionProvider connectionProvider)
        {
            db = databaseContext;
            dsClient = new DigitalstromWebserviceClient(connectionProvider);
        }

        // GET powerconsumption
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var powerValues = (await dsClient.GetTotalEnergy(60, 37)).TimeSeries
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