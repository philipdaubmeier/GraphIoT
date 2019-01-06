using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SmarthomeApi.Controllers
{
    [Route("powerconsumption")]
    public class PowerConsumptionController : Controller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // GET powerconsumption
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var definition = new
            {
                result = new
                {
                    meterID = new List<string>(),
                    type = "",
                    unit = "",
                    resolution = 60,
                    values = new List<List<double>>()
                },
                ok = true
            };
            var powerValuesFromDSS = await httpClient.GetStringAsync("***REMOVED***/json/metering/getValues%3Fdsuid%3D.meters%28all%29%26type%3Dconsumption%26resolution%3D60%26valueCount%3D37");
            var powerValuesRaw = JsonConvert.DeserializeAnonymousType(powerValuesFromDSS, definition);
            var powerValues = powerValuesRaw.result.values.Select(x => new KeyValuePair<DateTime, double>(UnixEpoch.AddSeconds(x.FirstOrDefault()), x.LastOrDefault())).OrderBy(x => x.Key).ToList();
            
            return Json(new
            { frames = new List<object>() {
                    new
                    {
                        text = Math.Round(powerValues.Last().Value, 0).ToString() + " W",
                        icon = "a21256"
                    },
                    new
                    {
                        index = 1,
                        chartData = powerValues.TakeLast(37).Select(x => (int)Math.Round(x.Value, 0))
                    }
                }
            });
        }
    }
}
