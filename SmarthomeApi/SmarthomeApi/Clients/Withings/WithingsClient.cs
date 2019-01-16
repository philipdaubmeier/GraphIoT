﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Withings
{
    public class WithingsClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private const string _clientId = "***REMOVED***";
        private const string _clientSecret = "***REMOVED***";
        public string ClientId { get { return _clientId; } }

        private string _refreshToken = "09af37ef1b0328ecb4ec77df2bcb3464ec40c102";

        private string _token = null;
        private DateTime _tokenValidTo = DateTime.MinValue;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public async Task<Dictionary<DateTime, int>> GetMeasures(int category)
        {
            await AuthenticateRefresh();
            return await ParseMeasuresResponse(await httpClient.GetAsync(
                new Uri("https://wbsapi.withings.net/measure?access_token=" + _token + "&action=getmeas&meastype=" + category.ToString())));
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
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("code", authorization_code),
                    new KeyValuePair<string, string>("redirect_uri", "https://your.domain/smarthome/api/withings/callback")
                })));

            _token = loadedToken.Item1;
            _tokenValidTo = loadedToken.Item2;
            _refreshToken = loadedToken.Item3;
            return _refreshToken;
        }

        /// <summary>
        /// Fetches a new access token via the refresh token. It is only requested if expired.
        /// </summary>
        private async Task AuthenticateRefresh()
        {
            if (_tokenValidTo > DateTime.Now && !string.IsNullOrEmpty(_token))
                return;
            
            Tuple<string, DateTime, string> loadedToken = await ParseTokenResponse(await httpClient.PostAsync(
                new Uri("https://account.withings.com/oauth2/token"), new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("refresh_token", _refreshToken)
                })));

            _token = loadedToken.Item1;
            _tokenValidTo = loadedToken.Item2;
            _refreshToken = loadedToken.Item3;
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
            var authRaw = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
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
            return measuresRaw.body.measuregrps.Select(x => new KeyValuePair<DateTime, int>(UnixEpoch.AddSeconds(x.date), x.measures.First().value)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
