using Newtonsoft.Json;
using SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Sonnen
{
    public class SonnenPortalClient
    {
        private const string _username = "demo";
        private const string _password = "demo";

        private static readonly HttpClient _client = new HttpClient();

        private const string _baseUri = @"https://meine.sonnenbatterie.de/";

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private TokenStore _tokenStore;

        public class EnergyStats
        {
            /// <summary>
            /// Discharge Value in Watts
            /// </summary>
            public int Discharge { get; set; }

            /// <summary>
            /// Charge Value in Watts
            /// </summary>
            public int Charge { get; set; }

            /// <summary>
            /// Photovoltaic production in Watts
            /// </summary>
            public int PvProduction { get; set; }

            /// <summary>
            /// Energy consumption in Watts
            /// </summary>
            public int Consumption { get; set; }

            /// <summary>
            /// Battery state of charge as an integer in percent (values 0-100)
            /// </summary>
            public int SOC { get; set; }
        }

        public SonnenPortalClient(PersistenceContext databaseContext)
        {
            _tokenStore = new TokenStore(databaseContext, "sonnen_portal");
        }

        public async Task<Dictionary<DateTime, EnergyStats>> GetEnergyStats(DateTime date)
        {
            await Authenticate();
            var uri = $"{_baseUri}historydata/basic_data?psb=demo&{(int)((date - UnixEpoch).TotalSeconds)}";
            return await ParseEnergyStats(await CallSonnenApi(new Uri(uri), _tokenStore.AccessToken));
        }

        private async Task Authenticate()
        {
            if (_tokenStore.IsAccessTokenValid())
                return;

            var response = await PostLogin();

            var cookies = response.Headers.GetValues("Set-Cookie").Select(x => x.Split("path=/").FirstOrDefault().Trim().TrimEnd(';')).ToList();
            var sessiontoken = string.Join(";", cookies);

            await _tokenStore.UpdateToken(sessiontoken, DateTime.Now.AddHours(1), string.Empty);
        }

        private async Task<HttpResponseMessage> PostLogin()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_baseUri}login"));
            AddCommonHeaders(request.Headers, $"{_baseUri}login");
            request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user", "demo"),
                    new KeyValuePair<string, string>("pass", "demo")
                });

            return await _client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> CallSonnenApi(Uri uri, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Accept", "application/json,text/javascript,*/*;q=0.01");
            AddCommonHeaders(request.Headers, $"{_baseUri}history/demo");
            request.Headers.Add("Cookie", token);

            return await _client.SendAsync(request);
        }

        private void AddCommonHeaders(HttpRequestHeaders headers, string referrer)
        {
            headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
            headers.Add("Connection", "keep-alive");
            headers.Add("Pragma", "no-cache");
            headers.Add("Cache-Control", "no-cache");
            headers.Referrer = new Uri(referrer);
            headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:60.0) Gecko/20100101 Firefox/60.0");
        }

        private async Task<Dictionary<DateTime, EnergyStats>> ParseEnergyStats(HttpResponseMessage response)
        {
            var definition = new List<List<int>>();
            var responseStr = await response.Content.ReadAsStringAsync();

            var energyStatsRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return energyStatsRaw.Select(item => new KeyValuePair<DateTime, EnergyStats>(
                UnixEpoch.AddSeconds(item[0]),
                new EnergyStats()
                {
                    Discharge = item[1],
                    Charge = item[2],
                    PvProduction = item[3],
                    Consumption = item[4],
                    SOC = item[5]
                }))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}