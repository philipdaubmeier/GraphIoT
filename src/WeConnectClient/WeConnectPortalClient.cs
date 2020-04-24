using PhilipDaubmeier.WeConnectClient.Network;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient
{
    public class WeConnectPortalClient : WeConnectAuthBase
    {
        private const string _baseUri = "https://www.portal.volkswagen-we.com";

        public WeConnectPortalClient(IWeConnectConnectionProvider<WeConnectPortalClient> connectionProvider)
            : base(connectionProvider) { }

        public async Task<HttpResponseMessage> GetEManager()
        {
            var uri = $"{_baseUri}/-/emanager/get-emanager";
            return await RequestApi(new Uri(uri));
        }
    }
}