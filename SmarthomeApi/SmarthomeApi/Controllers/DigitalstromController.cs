using System;
using System.Collections.Generic;
using System.Globalization;
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

        // GET: api/digitalstrom/sensors/curves/days/{day}
        [HttpGet("sensors/curves/days/{day}")]
        public ActionResult GetSensorsCurves([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode(404);

            List<DigitalstromZoneSensorData> sensorData = null; 
            db.Semaphore.WaitOne();
            try { sensorData = db.DsSensorDataSet.Where(x => x.Day == dayDate.Date).ToList(); }
            catch { throw; }
            finally { db.Semaphore.Release(); }
            if (sensorData == null)
                return StatusCode(404);

            return Json(new
            {
                zones = sensorData.Select(zone => new
                {
                    id = zone.ZoneId,
                    temperature_curve = zone.TemperatureSeries.ToList(0d),
                    humidity_curve = zone.HumiditySeries.ToList(0d)
                })
            });
        }

        // GET: api/digitalstrom/energy/curves/days/{day}
        [HttpGet("energy/curves/days/{day}")]
        public async Task<ActionResult> GetEnergyCurves([FromRoute] string day)
        {
            if (!DateTime.TryParseExact(day, "yyyy'-'MM'-'dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out DateTime dayDate))
                return StatusCode(404);

            DigitalstromEnergyHighresData energyData = null;
            db.Semaphore.WaitOne();
            try { energyData = db.DsEnergyHighresDataSet.Where(x => x.Day == dayDate.Date).FirstOrDefault(); }
            catch { throw; }
            finally { db.Semaphore.Release(); }
            if (energyData == null)
                return StatusCode(404);

            var circuitNames = await dsClient.GetMeteringCircuits();

            return Json(new
            {
                circuits = energyData.EnergySeriesEveryMeter.Select(circuit => new
                {
                    dsuid = circuit.Key.ToString(),
                    name = circuitNames.ContainsKey(circuit.Key) ? circuitNames[circuit.Key] : null,
                    energy_curve = circuit.Value.ToList(0)
                })
            });
        }
    }
}
