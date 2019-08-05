using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhilipDaubmeier.SonnenClient.Model;
using PhilipDaubmeier.SonnenClient.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SonnenClient
{
    public class SonnenPortalClient : SonnenAuthBase
    {
        private const string _baseUri = "https://my-api.sonnen.de/v1/";

        public SonnenPortalClient(ISonnenConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        /// <summary>
        /// Returns all site ids related to the currently signed in user.
        /// </summary>
        public async Task<List<string>> GetSites()
        {
            return await ParseSites(await RequestSonnenApi(new Uri($"{_baseUri}users/me")));
        }

        public async Task<MeasurementSeries> GetEnergyMeasurements(string siteId, DateTime start, DateTime end)
        {
            var query = $"?filter[start]={start.ToFilterTime()}&filter[end]={end.ToFilterTime()}&";
            var uri = new Uri($"{_baseUri}sites/{siteId}/measurements?{query}");
            return await CallSonnenApi<MeasurementWiremessage<MeasurementSeries>, MeasurementSeries>(uri);
        }

        private async Task<List<string>> ParseSites(HttpResponseMessage response)
        {
            var definition = new
            {
                data = new
                {
                    id = "",
                    type = "",
                    attributes = new
                    {
                        academic_title = "",
                        customer_number = "",
                        first_name = "",
                        last_name = "",
                        description = "",
                        email = "",
                        phone = "",
                        mobile = "",
                        street = "",
                        postal_code = "",
                        city = "",
                        state = "",
                        country_code = "",
                        latitude = "",
                        longitude = "",
                        language = "",
                        newsletter = true,
                        time_zone = "",
                        privacy_policy = "",
                        terms_of_service = "",
                        service_partners_data_access = true
                    },
                    relationships = new
                    {
                        sites = new
                        {
                            links = new
                            {
                                related = ""
                            },
                            data = new[]{ new
                            {
                                type = "",
                                id = ""
                            } }
                        }
                    },
                    links = new
                    {
                        self = ""
                    }
                }
            };
            var responseStr = await response.Content.ReadAsStringAsync();

            var sitesRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return sitesRaw.data.relationships.sites.data.Where(d => d.type.Equals("sites", StringComparison.InvariantCultureIgnoreCase))
                .Select(d => d.id).ToList();
        }
    }
}