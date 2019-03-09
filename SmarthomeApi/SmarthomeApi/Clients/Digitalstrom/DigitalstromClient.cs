using Newtonsoft.Json;
using SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Digitalstrom
{
    public class DigitalstromClient
    {
        private const string _username = "***REMOVED***";
        private const string _baseUrl = "https://ds-tools.net/cloudredir.php";
        private const string _dsCloudDssId = "***REMOVED***";
        private const string _token = "***REMOVED***";
        
        private static readonly HttpClient _client = new HttpClient();

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private TokenStore _tokenStore;

        public DigitalstromClient(PersistenceContext databaseContext)
        {
            _tokenStore = new TokenStore(databaseContext, "digitalstrom_dss");
        }

        public async Task<List<KeyValuePair<DateTime, double>>> GetTotalEnergy(int resolution, int count)
        {
            var path = $"/json/metering/getValues?dsuid=.meters(all)&type=consumption&resolution={resolution}&valueCount={count}";
            return await ParseTotalEnergyResponse(await GetDssPathAsync(path));
        }

        public async Task<Dictionary<Tuple<int, int>, Tuple<DateTime, double>>> GetSensorValues()
        {
            var path = "/json/property/query?query=/apartment/zones/*(ZoneID)/groups/group0/sensor/*(value,time,type)";
            return await ParseSensorValuesResponse(await GetDssPathAsync(path));
        }

        private async Task<HttpResponseMessage> GetDssPathAsync(string path)
        {
            var uri = new Uri($"{_baseUrl}?adr={_dsCloudDssId}&token={_token}&path={WebUtility.UrlEncode(path)}");

            return await _client.GetAsync(uri);
        }
        
        private async Task<List<KeyValuePair<DateTime, double>>> ParseTotalEnergyResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                result = new
                {
                    meterID = new List<string>(),
                    type = "",
                    unit = "",
                    resolution = 60,
                    values = new List<List<double>>()
                },
                ok = true
            };
            var responseStr = await response.Content.ReadAsStringAsync();
            var powerValuesRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return powerValuesRaw.result.values.Select(x => 
                new KeyValuePair<DateTime, double>(UnixEpoch.AddSeconds(x.FirstOrDefault()), x.LastOrDefault()))
                .OrderBy(x => x.Key).ToList();
        }

        private async Task<Dictionary<Tuple<int, int>, Tuple<DateTime, double>>> ParseSensorValuesResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                result = new
                {
                    zones = new[] { new
                    {
                        ZoneID = 0,
                        sensor = new[] { new
                        {
                            value = 0d,
                            time = 0,
                            type = 0
                        } }
                    } }
                },
                ok = true
            };
            var responseStr = await response.Content.ReadAsStringAsync();
            var sensorsRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return sensorsRaw.result.zones.SelectMany(x => x.sensor?.Select(s => 
                new Tuple<int, int, DateTime, double>(x.ZoneID, s.type, UnixEpoch.AddSeconds(s.time), s.value)) ?? new List<Tuple<int, int, DateTime, double>>())
                .ToDictionary(x => new Tuple<int, int>(x.Item1, x.Item2), x => new Tuple<DateTime, double>(x.Item3, x.Item4));
        }
    }
}
