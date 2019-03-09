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
    [Route("api/digitalstrom")]
    public class DigitalstromController : Controller
    {
        private static DigitalstromClient dsClient;

        private readonly PersistenceContext db;
        public DigitalstromController(PersistenceContext databaseContext)
        {
            db = databaseContext;
            dsClient = new DigitalstromClient(db);
        }

        // GET api/digitalstrom/sensors
        [HttpGet("sensors")]
        public async Task<JsonResult> GetSensors()
        {
            var sensorValues = (await dsClient.GetSensorValues()).GroupBy(x => x.Key.Item1);

            return Json(new
            {
                zones = sensorValues.Select(x => {
                    var temperatureVals = x.Where(s => s.Key.Item2 == 9).ToList();
                    var humidityVals = x.Where(s => s.Key.Item2 == 13).ToList();
                    return new
                    {
                        zone_id = x.Key,
                        temperature = temperatureVals.Count > 0 ? (double?)temperatureVals.First().Value.Item2 : null,
                        humidity = humidityVals.Count > 0 ? (double?)humidityVals.First().Value.Item2 : null
                    };
                })
            });
        }
    }
}
