using PhilipDaubmeier.WeConnectClient.Model.Emanager;
using PhilipDaubmeier.WeConnectClient.Network;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient
{
    public class WeConnectPortalClient : WeConnectAuthBase
    {
        private const string _baseUri = "https://www.portal.volkswagen-we.com";

        public WeConnectPortalClient(IWeConnectConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        public async Task<EmanagerResponse> GetEManager()
        {
            return await CallApi<EmanagerResponse>("/-/emanager/get-emanager");
        }
    }
}