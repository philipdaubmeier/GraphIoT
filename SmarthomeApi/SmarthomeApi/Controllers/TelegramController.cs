using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Clients.Netatmo;

namespace SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/telegram")]
    public class TelegramController : Controller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string chatId = "***REMOVED***";

        // GET: api/telegram/***REMOVED***/sendcamera
        [HttpGet("{bot}/sendcamera", Name = "sendcamera")]
        public async Task<JsonResult> sendcamera(string bot)
        {
            var picture = await NetatmoCameraImage.GetCurrent();
            await httpClient.GetStringAsync("https://api.telegram.org/" + bot + "/sendPhoto?chat_id=" + chatId + "&caption=Es+hat+geklingelt!&photo=" + WebUtility.UrlEncode(picture));

            return Json(new { result = "ok" });
        }
    }
}
