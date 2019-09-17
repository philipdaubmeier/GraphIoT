using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhilipDaubmeier.DigitalstromHost.Config;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.SmarthomeApi.Model.Config;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/telegram")]
    public class TelegramController : Controller
    {
        private NetatmoWebClient _netatmoClient;

        private static readonly HttpClient httpClient = new HttpClient();

        private IOptions<TelegramConfig> _config;
        private IOptions<DigitalstromConfig> _dsConfig;

        public TelegramController(IOptions<TelegramConfig> config, IOptions<DigitalstromConfig> dsConfig, NetatmoWebClient netatmoClient)
        {
            _config = config;
            _dsConfig = dsConfig;
            _netatmoClient = netatmoClient;
        }

        /// <summary>
        /// GET: api/telegram/{bot}/sendcamera
        /// 
        /// Sends the current camera picture and a caption to the bot chat
        /// </summary>
        [HttpGet("{bot}/sendcamera", Name = "sendcamera")]
        public async Task<JsonResult> sendcamera(string bot)
        {
            await sendTelegramPhoto(bot, _config.Value.ChatId, "Es hat geklingelt!", (await GetCameraPicture()).Item1);

            return Json(new { result = "ok" });
        }

        /// <summary>
        /// GET: api/telegram/{bot}/setwebhook
        /// 
        /// Call one single time to setup the telegram bot to call the update function
        /// </summary>
        [HttpGet("{bot}/setwebhook", Name = "setwebhook")]
        public async Task<JsonResult> setwebhook(string bot)
        {
            if (bot != _config.Value.BotId)
                return Json(new { result = "error", message = "This is not my bot!" });

            var url = _config.Value.WebhookBaseUri + "/api/telegram/" + bot + "/update";
            await httpClient.GetStringAsync("https://api.telegram.org/" + bot + "/setWebhook?url=" + WebUtility.UrlEncode(url));

            return Json(new { result = "ok" });
        }
        
        /// <summary>
        /// POST: api/telegram/{bot}/update
        /// 
        /// This gets called every time a message (or other update) occurs in the bot chat
        /// </summary>
        [HttpPost("{bot}/update", Name = "update")]
        public async Task<JsonResult> update(string bot)
        {
            if (bot != _config.Value.BotId)
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
                        await sendTelegramMessage(bot, _config.Value.ChatId, text);
                        break;
                    }
                case "g": goto case "garage";
                case "garage":
                    {
                        await sendTelegramMessage(bot, _config.Value.ChatId, "Meinst du die rechte oder linke Garage? [gr/gl]");
                        break;
                    }
                case "gl":
                    {
                        await sendTelegramMessage(bot, _config.Value.ChatId, "Ich triggere die linke Garage...");
                        await httpClient.GetStringAsync(new Uri(_dsConfig.Value.UriCloudredir, "/json/zone/callScene%3Fid%3D31990%26groupID%3D5%26sceneNumber%3D0%26category%3Dmanual"));
                        await sendTelegramPhoto(bot, _config.Value.ChatId, "Linke Garage getriggert...", (await GetCameraPicture()).Item2);
                        await Task.Delay(15000);
                        await sendTelegramMessage(bot, _config.Value.ChatId, "Stromverbrauch: xxx W");
                        await sendTelegramPhoto(bot, _config.Value.ChatId, "Linke Garage nach 15 Sekunden...", (await GetCameraPicture()).Item2);
                        break;
                    }
                case "gr":
                    {
                        await sendTelegramMessage(bot, _config.Value.ChatId, "Ich triggere die linke Garage...");
                        await httpClient.GetStringAsync(new Uri(_dsConfig.Value.UriCloudredir, "/json/zone/callScene%3Fid%3D31990%26groupID%3D4%26sceneNumber%3D0%26category%3Dmanual"));
                        await sendTelegramPhoto(bot, _config.Value.ChatId, "Rechte Garage getriggert...", (await GetCameraPicture()).Item2);
                        await Task.Delay(15000);
                        await sendTelegramMessage(bot, _config.Value.ChatId, "Stromverbrauch: xxx W");
                        await sendTelegramPhoto(bot, _config.Value.ChatId, "Rechte Garage nach 15 Sekunden...", (await GetCameraPicture()).Item2);
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
            if (rawData.message.from.first_name != _config.Value.WhitelistFirstname ||
                rawData.message.from.last_name != _config.Value.WhitelistLastname ||
                rawData.message.chat.id.ToString() != _config.Value.ChatId)
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

        private async Task<Tuple<string, string>> GetCameraPicture()
        {
            var rawData = await _netatmoClient.GetHomeData();
            var home = rawData.Homes.First(x => x.Name.Equals("Karlskron", StringComparison.InvariantCultureIgnoreCase));
            var camera = home.Cameras.First(x => x.Name.Equals("Phils Presence", StringComparison.InvariantCultureIgnoreCase));

            var events = home.Events.First(x => x.CameraId == camera.Id).EventList.Where(x => !string.IsNullOrWhiteSpace(x.Snapshot.Id) && !string.IsNullOrWhiteSpace(x.Snapshot.Key));
            var newestSnapshot = events.OrderByDescending(x => x.Time).First().Snapshot;
            var newestSnapshotUrl = "https://api.netatmo.com/api/getcamerapicture?image_id=" + newestSnapshot.Id + "&key=" + newestSnapshot.Key;

            var currentSnapshotUrl = camera.VpnUrl + "/live/snapshot_720.jpg";

            return new Tuple<string, string>(newestSnapshotUrl, currentSnapshotUrl);
        }
    }
}