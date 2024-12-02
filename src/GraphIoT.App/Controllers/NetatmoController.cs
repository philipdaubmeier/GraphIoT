using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Structure;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Model.Core;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Route("api/netatmo")]
    public class NetatmoController : Controller
    {
        private readonly NetatmoWebClient _netatmoClient;
        private readonly INetatmoDbContext _databaseContext;
        private readonly INetatmoDeviceService _netatmoStructure;

        public NetatmoController(NetatmoWebClient netatmoClient, INetatmoDbContext databaseContext, INetatmoDeviceService netatmoStructure)
        {
            _netatmoClient = netatmoClient;
            _databaseContext = databaseContext;
            _netatmoStructure = netatmoStructure;
        }

        // GET: api/netatmo/login
        [HttpGet("login")]
        public RedirectResult Login()
        {
            return Redirect(_netatmoClient.GetLoginUri().AbsoluteUri);
        }

        // GET: api/netatmo/authcallback?state=customState&code=owusQEHqhhosGHlod2oTuUXfxU9mMBtzurlyKK0IGjM
        [HttpGet("authcallback")]
        public async Task<ActionResult> AuthCallback(string state, string code)
        {
            if (!await _netatmoClient.TryCompleteLogin(state, code))
                return StatusCode((int)HttpStatusCode.Unauthorized);

            return Json(new
            {
                login = "success"
            });
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