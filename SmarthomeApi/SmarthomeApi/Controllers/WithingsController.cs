using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmarthomeApi.Clients.Withings;

namespace SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/withings")]
    public class WithingsController : Controller
    {
        private static readonly WithingsClient withingsClient = new WithingsClient();

        // GET: api/withings/login
        [HttpGet("login")]
        public void Login()
        {
            var scopes = new string[] { "user.info", "user.metrics", "user.activity" };

            Response.Redirect("https://account.withings.com/oauth2_user/authorize2?response_type=code&client_id=" + withingsClient.ClientId +
                "&state=f84b57ec&scope=" + string.Join(",", scopes) + "&redirect_uri=https://your.domain/smarthome/api/withings/callback");
        }

        // POST: api/withings/callback
        [HttpPost("callback")]
        [HttpGet("callback")]
        public async Task<IActionResult> PostCallback(string code, string state)
        {
            if (state != "f84b57ec")
                return StatusCode(403);

            var authorization_code = code;
            var refresh_token = await withingsClient.AuthenticateLogin(code);

            Func<string, Task> sendTelegramMessage = async message => await (new HttpClient()).GetStringAsync("https://api.telegram.org/***REMOVED***/sendMessage?chat_id=***REMOVED***&text=" + WebUtility.UrlEncode(message));
            await sendTelegramMessage("Refresh token: " + refresh_token);

            return StatusCode(200);
        }
        
        // HEAD: api/withings/callback
        [HttpHead("callback")]
        public IActionResult HeadCallback()
        {
            return StatusCode(200);
        }

        // GET: api/withings/measures
        [HttpGet("measures")]
        public async Task<JsonResult> Measures()
        {
            var measures = await withingsClient.GetMeasures(WithingsClient.MeasureType.Weight);

            return Json(new
            {
                measures = measures.OrderBy(x => x.Key).TakeLast(37).Select(x => Math.Round(((decimal)x.Value) / 1000, 2).ToString() + " kg")
            });
        }

        // GET: api/withings/devices
        [HttpGet("devices")]
        public async Task<JsonResult> Devices()
        {
            var devices = await withingsClient.GetDevices();

            return Json(new
            {
                devices = devices.Select(x => new
                {
                    model = x.Item1,
                    battery = x.Item2
                })
            });
        }
    }
}
