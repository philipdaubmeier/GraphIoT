using Microsoft.Extensions.Options;
using PhilipDaubmeier.SmarthomeApi.Model.Config;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Clients.Netatmo
{
    public class NetatmoCameraImage
    {
        private NetatmoWebserviceClient netatmoApiClient;

        public NetatmoCameraImage(IOptions<NetatmoConfig> config)
        {
            var netatmoAuthData = new NetatmoAuth()
            {
                NetatmoAppId = config.Value.NetatmoAppId,
                NetatmoAppSecret = config.Value.NetatmoAppSecret,
                Username = config.Value.Username,
                UserPassword = config.Value.Password
            };
            netatmoApiClient = new NetatmoWebserviceClient(new Uri("https://api.netatmo.net"), netatmoAuthData);
        }

        public async Task<string> GetLastEventSnapshot()
        {
            return (await netatmoApiClient.GetCameraPicture()).Item1;
        }

        public async Task<string> GetCurrentSnapshot()
        {
            return (await netatmoApiClient.GetCameraPicture()).Item2;
        }
    }
}