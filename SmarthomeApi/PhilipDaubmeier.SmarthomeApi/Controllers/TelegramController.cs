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
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhilipDaubmeier.SmarthomeApi.Clients.Netatmo;
using PhilipDaubmeier.SmarthomeApi.Model.Config;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/telegram")]
    public class TelegramController : Controller
    {
        private NetatmoCameraImage _netatmoClient;

        private static readonly HttpClient httpClient = new HttpClient();
        private const string mybot = "***REMOVED***";
        private const string chatId = "***REMOVED***";

        public TelegramController(IOptions<NetatmoConfig> config)
        {
            _netatmoClient = new NetatmoCameraImage(config);
        }

        /// <summary>
        /// GET: api/telegram/***REMOVED***/sendcamera
        /// 
        /// Sends the current camera picture and a caption to the bot chat
        /// </summary>
        [HttpGet("{bot}/sendcamera", Name = "sendcamera")]
        public async Task<JsonResult> sendcamera(string bot)
        {
            await sendTelegramPhoto(bot, chatId, "Es hat geklingelt!", await _netatmoClient.GetLastEventSnapshot());

            return Json(new { result = "ok" });
        }

        /// <summary>
        /// GET: api/telegram/***REMOVED***/setwebhook
        /// 
        /// Call one single time to setup the telegram bot to call the update function
        /// </summary>
        [HttpGet("{bot}/setwebhook", Name = "setwebhook")]
        public async Task<JsonResult> setwebhook(string bot)
        {
            if (bot != mybot)
                return Json(new { result = "error", message = "This is not my bot!" });

            var url = "***REMOVED***" + bot + "/update";
            await httpClient.GetStringAsync("https://api.telegram.org/" + bot + "/setWebhook?url=" + WebUtility.UrlEncode(url));

            return Json(new { result = "ok" });
        }
        
        /// <summary>
        /// POST: api/telegram/***REMOVED***/update
        /// 
        /// This gets called every time a message (or other update) occurs in the bot chat
        /// </summary>
        [HttpPost("{bot}/update", Name = "update")]
        public async Task<JsonResult> update(string bot)
        {
            if (bot != mybot)
                return Json(new { result = "error", message = "This is not my bot!" });

            string text;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                text = parseTelegramUpdate(await reader.ReadToEndAsync());

            if (text == null)
                return Json(new { result = "error", message = "Wrong update format or foreign person trying to control my bot!" });
            
            string command = text.ToLowerInvariant().Trim().Split(' ').First();
            switch (command)
            {
                case "echo":
                    {
                        await sendTelegramMessage(bot, chatId, text);
                        break;
                    }
                case "g": goto case "garage";
                case "garage":
                    {
                        await sendTelegramMessage(bot, chatId, "Meinst du die rechte oder linke Garage? [gr/gl]");
                        break;
                    }
                case "gl":
                    {
                        await sendTelegramMessage(bot, chatId, "Ich triggere die linke Garage...");
                        await httpClient.GetStringAsync("***REMOVED***/json/zone/callScene%3Fid%3D31990%26groupID%3D5%26sceneNumber%3D0%26category%3Dmanual");
                        await sendTelegramPhoto(bot, chatId, "Linke Garage getriggert...", await _netatmoClient.GetCurrentSnapshot());
                        await Task.Delay(15000);
                        await sendTelegramMessage(bot, chatId, "Stromverbrauch: xxx W");
                        await sendTelegramPhoto(bot, chatId, "Linke Garage nach 15 Sekunden...", await _netatmoClient.GetCurrentSnapshot());
                        break;
                    }
                case "gr":
                    {
                        await sendTelegramMessage(bot, chatId, "Ich triggere die linke Garage...");
                        await httpClient.GetStringAsync("***REMOVED***/json/zone/callScene%3Fid%3D31990%26groupID%3D4%26sceneNumber%3D0%26category%3Dmanual");
                        await sendTelegramPhoto(bot, chatId, "Rechte Garage getriggert...", await _netatmoClient.GetCurrentSnapshot());
                        await Task.Delay(15000);
                        await sendTelegramMessage(bot, chatId, "Stromverbrauch: xxx W");
                        await sendTelegramPhoto(bot, chatId, "Rechte Garage nach 15 Sekunden...", await _netatmoClient.GetCurrentSnapshot());
                        break;
                    }
            }

            return Json(new { result = "ok" });
        }

        public string parseTelegramUpdate(string raw)
        {
            var definition = new
            {
                update_id = 0,
                message = new
                {
                    message_id = 0,
                    from = new
                    {
                        id = 0,
                        is_bot = false,
                        first_name = "",
                        last_name = "",
                        language_code = ""
                    },
                    chat = new
                    {
                        id = 0,
                        first_name = "",
                        last_name = "",
                        type = ""
                    },
                    date = 0,
                    text = ""
                }
            };
            var rawData = JsonConvert.DeserializeAnonymousType(raw, definition);
            if (rawData.message.from.first_name != "Philip" ||
                rawData.message.from.last_name != "Daubmeier" ||
                rawData.message.chat.id.ToString() != chatId)
                return null;

            return rawData.message.text;
        }

        private static async Task sendTelegramMessage(string bot, string chat, string message)
        {
            await httpClient.GetStringAsync("https://api.telegram.org/" + bot + "/sendMessage?chat_id=" + chat + "&text=" + WebUtility.UrlEncode(message));
        }

        private static async Task sendTelegramPhoto(string bot, string chat, string caption, string picture)
        {
            await httpClient.GetStringAsync("https://api.telegram.org/" + bot + "/sendPhoto?chat_id=" + chat + "&caption=" + WebUtility.UrlEncode(caption) + "&photo=" + WebUtility.UrlEncode(picture));
        }
    }
}
