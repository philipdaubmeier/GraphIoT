using Newtonsoft.Json;
using PhilipDaubmeier.ViessmannClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient
{
    public class ViessmannEstrellaClient
    {
        private readonly IViessmannConnectionProvider<ViessmannEstrellaClient> _connectionProvider;

        private HttpClient _client = new HttpClient();

        public ViessmannEstrellaClient(IViessmannConnectionProvider<ViessmannEstrellaClient> connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<List<int>> GetGateways()
        {
            return await ParseGatewaysResponse(await GetAsync(
                new Uri("https://api.viessmann.io/estrella/rest/v2.0/gateways/")));
        }

        public async Task<List<Tuple<int, string>>> GetControllers(int gatewayId)
        {
            return await ParseControllersResponse(await GetAsync(
                new Uri($"https://api.viessmann.io/estrella/rest/v2.0/gateways/{gatewayId}/controllers")));
        }
        
        private async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };

            var username = _connectionProvider.AuthData.Username;
            var password = _connectionProvider.AuthData.UserPassword;
            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            
            return await _client.SendAsync(request);
        }

        private async Task<List<int>> ParseGatewaysResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                data = new[] { new
                {
                    id = "",
                    type = "",
                    ownerId = "",
                    name = "",
                    gatewaySerial = "",
                    macAddress = "",
                    street = "",
                    houseNumber = "",
                    zip = "",
                    city = "",
                    country = "",
                    region = "",
                    trackingNumber = "",
                    isRegistered = true,
                    isActive = true,
                    status = "",
                    autoUpdate = false,
                    firmwareVersion = "",
                    hasErrors = false,
                    producedAt = "",
                    lastCommunicationAt = "",
                    connectedAt = "",
                    disconnectedAt = "",
                    registeredAt = "",
                    createdAt = "",
                    editedAt = "",
                    latitude = 0d,
                    longitude = 0d,
                    timeZone = "",
                    equipmentNumber = "",
                    isDeleted = false,
                    wifiStrength = 0
                } },
                paging = new
                {
                    offset = 0,
                    limit = 0,
                    total = 0
                }
            };
            var gatewaysRaw = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
            return gatewaysRaw.data.Select(x => int.Parse(x.id)).ToList();
        }

        private async Task<List<Tuple<int, string>>> ParseControllersResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                data = new[] { new
                {
                    id = "",
                    boilerSerial = "",
                    bmuSerial = "",
                    typeId = "",
                    typeName = "",
                    hasError = false,
                    isActive = true,
                    gatewayId = "",
                    categoryFamilyIndex = 0,
                    status = "",
                    synchronizationStatus = ""
                } }
            };
            var controllersRaw = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
            return controllersRaw.data.Select(x => new Tuple<int, string>(int.Parse(x.id), x.typeName)).ToList();
        }
    }
}