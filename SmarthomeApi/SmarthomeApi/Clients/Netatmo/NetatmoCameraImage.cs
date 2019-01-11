using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Netatmo
{
    public class NetatmoCameraImage
    {
        public static async Task<string> GetCurrent()
        {
            var netatmoAuthData = new NetatmoAuth()
            {
                NetatmoAppId = "***REMOVED***",
                NetatmoAppSecret = "***REMOVED***",
                Username = "***REMOVED***",
                UserPassword = "***REMOVED***"
            };

            var netatmoApiClient = new NetatmoWebserviceClient(new Uri("https://api.netatmo.net"), netatmoAuthData);
            var pictureId = await netatmoApiClient.GetCameraPicture();

            return "https://api.netatmo.com/api/getcamerapicture?image_id=" + pictureId.Item1 + "&key=" + pictureId.Item2;
        }
    }
}
