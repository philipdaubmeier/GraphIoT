using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhilipDaubmeier.SmarthomeApi.Clients.AudiConnect;
using PhilipDaubmeier.SmarthomeApi.Model.Config;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/audiconnect")]
    public class AudiConnectController : Controller
    {
        private IOptions<AudiConnectConfig> _config;
        public AudiConnectController(IOptions<AudiConnectConfig> config)
        {
            _config = config;
        }

        // GET: api/audiconnect/vehicles
        [HttpGet("vehicles")]
        public async Task<JsonResult> vehicles()
        {
            AudiConnectClient client = new AudiConnectClient(_config);
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

        // GET: api/audiconnect/vehicles/pairing
        [HttpGet("vehicles/pairing")]
        public async Task<JsonResult> vehiclesPairing()
        {
            AudiConnectClient client = new AudiConnectClient(_config);
            var vehicles = await client.GetVehicles();
            var pairings = (await Task.WhenAll(vehicles.Select(async x => await client.GetPairingStatus(x.Value)).ToList()))
                .Zip(vehicles, (x1, x2) => new Tuple<string, string>(x2.Value, x1));

            return Json(new
            {
                vehicles = pairings.Select(vehicle => new
                {
                    vin = vehicle.Item1,
                    pairing = JsonConvert.DeserializeObject(vehicle.Item2)
                })
            });
        }

        // GET: api/audiconnect/vehicles/vsr
        [HttpGet("vehicles/vsr")]
        public async Task<JsonResult> vehiclesVsr()
        {
            AudiConnectClient client = new AudiConnectClient(_config);
            var vehicles = await client.GetVehicles();
            var pairings = (await Task.WhenAll(vehicles.Select(async x => await client.GetVsrStoredData(x.Value)).ToList()))
                .Zip(vehicles, (x1, x2) => new Tuple<string, string>(x2.Value, x1));

            return Json(new
            {
                vehicles = pairings.Select(vehicle => new
                {
                    vin = vehicle.Item1,
                    vsr = JsonConvert.DeserializeObject(vehicle.Item2)
                })
            });
        }

        // GET: api/audiconnect/vehicles/operations
        [HttpGet("vehicles/operations")]
        public async Task<JsonResult> vehiclesOperations()
        {
            AudiConnectClient client = new AudiConnectClient(_config);
            var vehicles = await client.GetVehicles();
            var pairings = (await Task.WhenAll(vehicles.Select(async x => await client.GetOperationList(x.Value)).ToList()))
                .Zip(vehicles, (x1, x2) => new Tuple<string, string>(x2.Value, x1));

            return Json(new
            {
                vehicles = pairings.Select(vehicle => new
                {
                    vin = vehicle.Item1,
                    operation_list = JsonConvert.DeserializeObject(vehicle.Item2)
                })
            });
        }
    }
}
