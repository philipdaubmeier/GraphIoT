using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Structure;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System.Linq;
using System.Net;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Route("api/netatmo")]
    public class NetatmoController : Controller
    {
        private readonly INetatmoDbContext _databaseContext;
        private readonly INetatmoDeviceService _netatmoStructure;

        public NetatmoController(INetatmoDbContext databaseContext, INetatmoDeviceService netatmoStructure)
        {
            _databaseContext = databaseContext;
            _netatmoStructure = netatmoStructure;
        }

        // DELETE api/netatmo/structure/modules/{moduleId}
        [HttpDelete("structure/modules/{moduleId}")]
        public ActionResult DeleteModule([FromRoute] string moduleId)
        {
            var normalizedModuleId = (string)(ModuleId)moduleId;
            _databaseContext.NetatmoModuleMeasures.RemoveRange(_databaseContext.NetatmoModuleMeasures.Where(x => x.ModuleId == normalizedModuleId));
            _databaseContext.SaveChanges();
            return StatusCode((int)HttpStatusCode.OK);
        }

        // POST api/netatmo/structure/reload
        [HttpPost("structure/reload")]
        public ActionResult ReloadStructure()
        {
            _netatmoStructure.ReloadFromNetatmoApi();
            return StatusCode((int)HttpStatusCode.OK);
        }
    }
}