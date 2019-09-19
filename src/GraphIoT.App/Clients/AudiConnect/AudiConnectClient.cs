using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhilipDaubmeier.GraphIoT.App.Model.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.App.Clients.AudiConnect
{
    public class AudiConnectClient
    {
        private IOptions<AudiConnectConfig> _config;

        private string _token = null;
        private DateTime _tokenValidTo = DateTime.MinValue;

        public AudiConnectClient(IOptions<AudiConnectConfig> config)
        {
            _config = config;
        }

        /// <summary>
        /// Returns a dictionary containing CSIDs as keys and VINs as values
        /// </summary>
        public async Task<Dictionary<string, string>> GetVehicles()
        {
            await Authenticate();
            return await ParseVehiclesResponse(await SendAsync(HttpMethod.Get,
                new Uri("https://msg.audi.de/fs-car/myaudi/carservice/v2/Audi/DE/vehicles")));
        }

        /// <summary>
        /// Returns vehicle details for a CSID in a tuple of name, modelcode, colorname and list of PR numbers
        /// </summary>
        public async Task<Tuple<string, string, string, List<string>>> GetVehicleDetails(string csid)
        {
            await Authenticate();
            return await ParseVehicleDetailResponse(await SendAsync(HttpMethod.Get,
                new Uri("https://msg.audi.de/fs-car/myaudi/carservice/v2/Audi/DE/vehicle/" + csid)));
        }
        
        public async Task<string> GetPairingStatus(string vin)
        {
            await Authenticate();
            return await (await SendAsync(HttpMethod.Get,
                new Uri("https://msg.audi.de/fs-car/usermanagement/users/v1/Audi/DE/vehicles/" + vin + "/pairing")))
                .Content.ReadAsStringAsync();
        }

        public async Task<string> GetVsrStoredData(string vin)
        {
            await Authenticate();
            return await (await SendAsync(HttpMethod.Get,
                new Uri("https://msg.audi.de/fs-car/bs/vsr/v1/Audi/DE/vehicles/" + vin + "/status")))
                .Content.ReadAsStringAsync();
        }

        public async Task<string> GetOperationList(string vin)
        {
            await Authenticate();
            return await (await SendAsync(HttpMethod.Get,
                new Uri("https://msg.audi.de/fs-car/rolesrights/operationlist/v2/Audi/DE/vehicles/" + vin + "/operations")))
                .Content.ReadAsStringAsync();
        }

        private async Task Authenticate()
        {
            if (_tokenValidTo > DateTime.Now && !string.IsNullOrEmpty(_token))
                return;

            Tuple<string, DateTime> loadedToken = await ParseTokenResponse(await SendAsync(HttpMethod.Post,
                new Uri("https://msg.audi.de/fs-car/core/auth/v1/Audi/DE/token"), new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", _config.Value.Username),
                    new KeyValuePair<string, string>("password", _config.Value.Password)
                })));

            _token = loadedToken.Item1;
            _tokenValidTo = loadedToken.Item2;
        }
        
        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, Uri uri, FormUrlEncodedContent postData = null)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = method,
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("X-App-ID", "de.audi.mmiapp");
            request.Headers.Add("X-App-Name", "MMIconnect");
            request.Headers.Add("X-App-Version", "2.8.3");
            request.Headers.Add("X-Brand", "audi");
            request.Headers.Add("X-Country-Id", "DE");
            request.Headers.Add("X-Language-Id", "de");
            request.Headers.Add("X-Platform", "google");
            request.Headers.Add("User-Agent", "okhttp/2.7.4");
            request.Headers.Add("ADRUM_1", "isModule:true");
            request.Headers.Add("ADRUM", "isAray:true");
            request.Headers.Add("Authorization", "AudiAuth 1" + ((string.IsNullOrEmpty(_token) || _tokenValidTo < DateTime.Now) ? "" : " " + _token));
            if (postData != null)
                request.Content = postData;
            return await client.SendAsync(request);
        }

        private async Task<Tuple<string, DateTime>> ParseTokenResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                access_token = "",
                token_type = "",
                expires_in = 0
            };
            var rawData = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
            return new Tuple<string, DateTime>(rawData.access_token, DateTime.Now.AddSeconds(rawData.expires_in));
        }

        private async Task<Dictionary<string, string>> ParseVehiclesResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                getUserVINsResponse = new
                {
                    CSIDVins = new[] { new
                    {
                        CSID = "",
                        VIN = "",
                        registered = ""
                    } },
                    vinsOnBlacklist = 0
                }
            };
            var rawData = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
            return rawData.getUserVINsResponse.CSIDVins.ToDictionary(x => x.CSID, x => x.VIN);
        }

        private async Task<Tuple<string, string, string, List<string>>> ParseVehicleDetailResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                getVehicleDataResponse = new
                {
                    Vehicle = new
                    {
                        LifeData = new
                        {
                            MediaData = new[] { new
                            {
                                MediaType = "",
                                URL = ""
                            } }
                        }
                    },
                    VehicleSpecification = new
                    {
                        ModelCoding = new
                        {
                            name = "",
                            value = ""
                        },
                        ColourInterior = new
                        {
                            ColourSalesFamilies = new
                            {
                                ColourSalesFamilies = new
                                {
                                    Finish = new
                                    {
                                        ColourCode = new
                                        {
                                            name = "",
                                            value = ""
                                        }
                                    }
                                }
                            }
                        },
                        EquipmentSalesFamilies = new
                        {
                            EquipmentFamilies = new[] { new
                            {
                                EquipmentChoice = new
                                {
                                    BASYS = new
                                    {
                                        Code = ""
                                    }
                                }
                            } }
                        }
                    }
                }
            };
            var rawData = JsonConvert.DeserializeAnonymousType((await response.Content.ReadAsStringAsync()).Replace("\"@name\":", "\"name\":"), definition);
            return new Tuple<string, string, string, List<string>>(rawData.getVehicleDataResponse.VehicleSpecification.ModelCoding.name,
                rawData.getVehicleDataResponse.VehicleSpecification.ModelCoding.value,
                rawData.getVehicleDataResponse.VehicleSpecification.ColourInterior.ColourSalesFamilies.ColourSalesFamilies.Finish.ColourCode.name,
                rawData.getVehicleDataResponse.VehicleSpecification.EquipmentSalesFamilies.EquipmentFamilies.Select(x => x.EquipmentChoice.BASYS.Code).ToList());
        }
    }
}
