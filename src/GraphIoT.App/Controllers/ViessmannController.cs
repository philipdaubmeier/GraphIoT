using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.ViessmannClient;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Produces("application/json")]
    [Route("api/viessmann")]
    public class ViessmannController : Controller
    {
        private readonly ViessmannPlatformClient _viessmannClient;

        public ViessmannController(ViessmannPlatformClient platformClient)
        {
            _viessmannClient = platformClient;
        }

        // GET: api/viessmann/installations
        [HttpGet("installations")]
        public async Task<JsonResult> GetInstallations()
        {
            var res = await _viessmannClient.GetInstallations();

            return Json(new
            {
                installations = res.Data.Select(x => new
                {
                    id = x.Id,
                    desc = x.Description
                })
            });
        }

        // GET: api/viessmann/installations/{installationId}/gateways
        [HttpGet("installations/{installationId}/gateways")]
        public async Task<JsonResult> GetGateways(long installationId)
        {
            var gateways = await _viessmannClient.GetGateways(installationId);

            return Json(new
            {
                gateways = gateways.Data.Select(x => new
                {
                    id = x.Id,
                    type = x.GatewayType,
                    version = x.Version
                })
            });
        }

        // GET: api/viessmann/installations/{installationId}/gateways/{gatewayId}/devices
        [HttpGet("installations/{installationId}/gateways/{gatewayId}/devices")]
        public async Task<JsonResult> GetDevices(long installationId, long gatewayId)
        {
            var devices = await _viessmannClient.GetDevices(installationId, gatewayId);

            return Json(new
            {
                devices = devices.Data.Select(x => new
                {
                    id = x.Id,
                    type = x.DeviceType,
                    model = x.ModelId
                })
            });
        }
        
        // GET: api/viessmann/installations/{installationId}/gateways/{gatewayId}/devices/{deviceId}/features
        [HttpGet("installations/{installationId}/gateways/{gatewayId}/devices/{deviceId}/features")]
        public async Task<JsonResult> GetFeatures(long installationId, long gatewayId, long deviceId)
        {
            var features = await _viessmannClient.GetFeatures(installationId, gatewayId, deviceId);
            
            return Json(new
            {
                features = features.Features.Select(x=>new
                {
                    name = x.Name,
                    properties = x.Properties?.GetProperties().Select(p => new
                    {
                        name = p.property,
                        p.value
                    })
                })
            });
        }
    }
}