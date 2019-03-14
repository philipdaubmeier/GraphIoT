using CompactTimeSeries;
using Newtonsoft.Json;
using SmarthomeApi.Database.Model;
using SmarthomeApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
            return await GetEnergy(".meters(all)", resolution, count);
        }

        public async Task<List<KeyValuePair<DateTime, double>>> GetEnergy(string meterDsuid, int resolution, int count)
        {
            var path = $"/json/metering/getValues?dsuid={meterDsuid}&type=consumption&resolution={resolution}&valueCount={count}";
            return await ParseTotalEnergyResponse(await GetDssPathAsync(path));
        }

        public async Task ReadEnergyToTimeSeries(string meterDsuid, int resolution, int count, ITimeSeries<int> timeseries)
        {
            var path = $"/json/metering/getValues?dsuid={meterDsuid}&type=consumption&resolution={resolution}&valueCount={count}";
            await ParseTotalEnergyResponseIntoTimeSeries(await GetDssPathAsync(path), timeseries);
        }

        public async Task<Dictionary<string, string>> GetMeteringCircuits()
        {
            var path = "/json/property/query?query=/apartment/dSMeters/*(dSUID,name)/capabilities(metering)";
            return await ParseMeteringCircuitsResponse(await GetDssPathAsync(path));
        }

        public async Task<Dictionary<string, List<int>>> GetCircuitZones()
        {
            var path = "/json/property/query?query=/apartment/dSMeters/*(dSUID)/zones/*(ZoneID)";
            return await ParseCircuitZonesResponse(await GetDssPathAsync(path));
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

        private async Task ParseTotalEnergyResponseIntoTimeSeries(HttpResponseMessage response, ITimeSeries<int> timeseries)
        {
            var definition = new
            {
                result = new
                {
                    type = "",
                    unit = "",
                    resolution = 60,
                    values = new List<List<double>>()
                },
                ok = true
            };

            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(await response.Content.ReadAsStreamAsync()))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var powerValuesRaw = CastToAnonymousType(definition, serializer.Deserialize(jsonTextReader, definition.GetType()));
                foreach (var value in powerValuesRaw.result.values)
                    timeseries[UnixEpoch.AddSeconds(value.FirstOrDefault()).ToLocalTime()] = (int)value.LastOrDefault();
            }
        }

        private static T CastToAnonymousType<T>(T typeHolder, object x)
        {
            return (T)x;
        }

        private async Task<Dictionary<string, List<int>>> ParseCircuitZonesResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                result = new
                {
                    dSMeters = new[] { new
                    {
                        dSUID = "",
                        zones = new[] { new
                        {
                            ZoneID = 0
                        } }
                    } }
                },
                ok = true
            };
            var responseStr = await response.Content.ReadAsStringAsync();
            var circuitValuesRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return circuitValuesRaw.result.dSMeters.Select(x =>
                new KeyValuePair<string, List<int>>(x.dSUID, x.zones.Select(z => z.ZoneID).ToList()))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<Dictionary<string, string>> ParseMeteringCircuitsResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                result = new
                {
                    dSMeters = new[] { new
                    {
                        dSUID = "",
                        name = "",
                        capabilities = new[] { new
                        {
                            metering = false
                        } }
                    } }
                },
                ok = true
            };
            var responseStr = await response.Content.ReadAsStringAsync();
            var circuitValuesRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return circuitValuesRaw.result.dSMeters.Where(x => x.capabilities.FirstOrDefault()?.metering ?? false)
                .Select(x => new KeyValuePair<string, string>(x.dSUID, x.name))
                .ToDictionary(x => x.Key, x => x.Value);
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
