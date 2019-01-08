using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/withings")]
    public class WithingsController : Controller
    {
        private const string clientId = "***REMOVED***";
        private const string consumerSecret = "***REMOVED***";

        // POST: api/withings/callback
        [HttpPost("callback")]
        public IActionResult PostCallback([FromForm]string formdata, [FromBody]string body)
        {
            return StatusCode(200);
        }

        // HEAD: api/withings/callback
        [HttpHead("callback")]
        public IActionResult HeadCallback()
        {
            return StatusCode(200);
        }
    }
}
