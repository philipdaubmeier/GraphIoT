using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NodaTime;
using PhilipDaubmeier.SmarthomeApi.Model.Config;
using PhilipDaubmeier.TokenStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Clients.Withings
{
    public class WithingsClient
    {
        private readonly IOptions<WithingsConfig> _config;

        private static readonly HttpClient httpClient = new HttpClient();

        private const string _baseUri = "https://wbsapi.withings.net";
        
        public string ClientId { get { return _config.Value.WithingsClientId; } }

        private TokenStore<WithingsClient> _tokenStore;

        public enum MeasureType
        {
            Weight      = 1,  /// <summery>Weight (kg)</summary>
            Height      = 4,  /// <summary>Height (meter)</summary>
            FatFreeMass = 5,  /// <summary>Fat Free Mass (kg)</summary>
            FatRatio    = 6,  /// <summary>Fat Ratio (%)</summary>
            FatMass     = 8,  /// <summary>Fat Mass Weight (kg)</summary>
            Diastolic   = 9,  /// <summary>Diastolic Blood Pressure (mmHg)</summary>
            Systolic    = 10, /// <summary>Systolic Blood Pressure (mmHg)</summary>
            HeartPulse  = 11, /// <summary>Heart Pulse (bpm)</summary>
            Temperature = 12, /// <summary>Temperature</summary>
            SP02        = 54, /// <summary>SP02 (%)</summary>
            BodyTemp    = 71, /// <summary>Body Temperature</summary>
            SkinTemp    = 73, /// <summary>Skin Temperature</summary>
            MuscleMass  = 76, /// <summary>Muscle Mass</summary>
            Hydration   = 77, /// <summary>Hydration</summary>
            BoneMass    = 88, /// <summary>Bone Mass</summary>
            PulseWave   = 91  /// <summary>Pulse Wave Velocity</summary>
        }

        public WithingsClient(TokenStore<WithingsClient> tokenStore, IOptions<WithingsConfig> config)
        {
            _tokenStore = tokenStore;
            _config = config;
        }

        public async Task<Dictionary<DateTime, int>> GetMeasures(MeasureType type)
        {
            await AuthenticateRefresh();
            return await ParseMeasuresResponse(await httpClient.GetAsync(
                new Uri($"{_baseUri}/measure?access_token={_tokenStore.AccessToken}&action=getmeas&meastype={((int)type).ToString()}")));
        }

        public async Task<List<Tuple<string, string>>> GetDevices()
        {
            await AuthenticateRefresh();
            return await ParseDevicesResponse(await httpClient.GetAsync(
                new Uri($"{_baseUri}/v2/user?action=getdevice&access_token={_tokenStore.AccessToken}")));
        }

        /// <summary>
        /// Authenticate via authorization code. Sets the access token and returns the refresh token.
        /// </summary>
        /// <returns></returns>
        public async Task<string> AuthenticateLogin(string authorization_code)
        {
            Tuple<string, DateTime, string> loadedToken = await ParseTokenResponse(await httpClient.PostAsync(
                new Uri("https://account.withings.com/oauth2/token"), new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _config.Value.WithingsClientId),
                    new KeyValuePair<string, string>("client_secret", _config.Value.WithingsClientSecret),
                    new KeyValuePair<string, string>("code", authorization_code),
                    new KeyValuePair<string, string>("redirect_uri", "https://your.domain/smarthome/api/withings/callback")
                })));

            await _tokenStore.UpdateToken(loadedToken.Item1, loadedToken.Item2, loadedToken.Item3);
            return loadedToken.Item3;
        }

        /// <summary>
        /// Fetches a new access token via the refresh token. It is only requested if expired.
        /// </summary>
        private async Task AuthenticateRefresh()
        {
            if (_tokenStore.IsAccessTokenValid())
                return;
            
            Tuple<string, DateTime, string> loadedToken = await ParseTokenResponse(await httpClient.PostAsync(
                new Uri("https://account.withings.com/oauth2/token"), new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", _config.Value.WithingsClientId),
                    new KeyValuePair<string, string>("client_secret", _config.Value.WithingsClientSecret),
                    new KeyValuePair<string, string>("refresh_token", _tokenStore.RefreshToken)
                })));

            await _tokenStore.UpdateToken(loadedToken.Item1, loadedToken.Item2, loadedToken.Item3);
        }

        /// <summary>
        /// Returns the access token, the expiry time and the refresh token in the returned tuple
        /// </summary>
        private async Task<Tuple<string, DateTime, string>> ParseTokenResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                access_token = "",
                expires_in = "",
                token_type = "",
                scope = "",
                refresh_token = "",
                userid = ""
            };
            var responseStr = await response.Content.ReadAsStringAsync();
            var authRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return new Tuple<string, DateTime, string>(authRaw.access_token, DateTime.Now.AddSeconds(int.Parse(authRaw.expires_in)), authRaw.refresh_token);
        }

        private async Task<Dictionary<DateTime, int>> ParseMeasuresResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                status = 0,
                body = new
                {
                    updatetime = 0,
                    timezone = "",
                    measuregrps = new[] { new
                    {
                        grpid = 0,
                        attrib = 0,
                        date = 0,
                        created = 0,
                        category = 0,
                        deviceid = "",
                        measures = new[] { new
                        {
                            value = 0,
                            type = 0,
                            unit = 0,
                            algo = 0,
                            fw = 0,
                            fm = 0
                        } },
                        comment = ""
                    } }
                }
            };
            var measuresRaw = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
            return measuresRaw.body.measuregrps.Select(x => new KeyValuePair<DateTime, int>(Instant.FromUnixTimeSeconds(x.date).ToDateTimeUtc(), x.measures.First().value)).ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<List<Tuple<string, string>>> ParseDevicesResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                status = 0,
                body = new
                {
                    devices = new[] { new
                    {
                        type = "",
                        battery = "",
                        model = "",
                        deviceid = "",
                        timezone = ""
                    } }
                }
            };
            var devicesRaw = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
            return devicesRaw.body.devices.Select(x => new Tuple<string, string>(x.model, x.battery)).ToList();
        }
    }
}