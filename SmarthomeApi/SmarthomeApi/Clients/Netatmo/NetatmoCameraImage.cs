using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Netatmo
{
    public class NetatmoCameraImage
    {
        private static NetatmoAuth netatmoAuthData = new NetatmoAuth()
        {
            NetatmoAppId = "***REMOVED***",
            NetatmoAppSecret = "***REMOVED***",
            Username = "***REMOVED***",
            UserPassword = "***REMOVED***"
        };

        private static NetatmoWebserviceClient netatmoApiClient = new NetatmoWebserviceClient(new Uri("https://api.netatmo.net"), netatmoAuthData);

        public static async Task<string> GetLastEventSnapshot()
        {
            return (await netatmoApiClient.GetCameraPicture()).Item1;
        }

        public static async Task<string> GetCurrentSnapshot()
        {
            return (await netatmoApiClient.GetCameraPicture()).Item2;
        }
    }
}
