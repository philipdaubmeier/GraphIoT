using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Clients.AudiConnect;

namespace SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/audiconnect")]
    public class AudiConnectController : Controller
    {
        // GET: api/audiconnect/vehicles
        [HttpGet("vehicles")]
        public async Task<JsonResult> vehicles()
        {
            AudiConnectClient client = new AudiConnectClient();
            var vehicles = await client.GetVehicles();
            var details = (await Task.WhenAll(vehicles.Select(async x => await client.GetVehicleDetails(x.Key)).ToList()))
                .Zip(vehicles, (x1, x2) => new Tuple<KeyValuePair<string, string>, Tuple<string, string, string, List<string>>>(x2, x1));

            return Json(new
            {
                vehicles = details.Select(vehicle => new
                {
                    vin = vehicle.Item1.Value,
                    name = vehicle.Item2.Item1,
                    modelcode = vehicle.Item2.Item2,
                    color = vehicle.Item2.Item3,
                    pr_numbers = vehicle.Item2.Item4
                })
            });
        }
    }
}
