using Newtonsoft.Json;
using SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Viessmann
{
    public class ViessmannPlatformClient
    {
        private const string _username = "***REMOVED***";
        private const string _password = "***REMOVED***";
        private const string _installationId = "***REMOVED***";
        private const string _gatewayId = "***REMOVED***";

        private static readonly HttpClientHandler _authClientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
        private static readonly HttpClient _authClient = new HttpClient(_authClientHandler);
        private static readonly HttpClient _client = new HttpClient();

        private const string _authUri = "https://iam.viessmann.com/idp/v1/authorize";
        private const string _tokenUri = "https://iam.viessmann.com/idp/v1/token";
        private const string _clientId = "***REMOVED***";
        private const string _clientSecret = "***REMOVED***";
        private const string _redirectUri = "vicare://oauth-callback/everest";

        private TokenStore _tokenStore;

        public ViessmannPlatformClient(PersistenceContext databaseContext)
        {
            _tokenStore = new TokenStore(databaseContext, "viessmann_platform");
        }

        public async Task<string> GetInstallations()
        {
            await Authenticate();

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.viessmann-platform.io/general-management/v1/installations?expanded=true"),
                Method = HttpMethod.Get
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);

            return await (await _authClient.SendAsync(request)).Content.ReadAsStringAsync();
        }

        public async Task<double> GetBoilerTemperature()
        {
            return ((double?)await ParseFeatureResponse<ClassNullable<double>>(await GetFeature("heating.boiler.temperature"), "value")) ?? double.NaN;
        }

        public async Task<Dictionary<string, string>> GetFeatures()
        {
            var results = new Dictionary<string, string>();

            await GetFeature<string, ClassNullable<double>>(results, "heating.sensors.temperature.outside", "status", "value");
            await GetFeature<ClassNullable<double>>(results, "heating.boiler.temperature", "value");
            await GetFeature<ClassNullable<bool>>(results, "heating.burner", "active");
            await GetFeature<ClassNullable<double>, ClassNullable<double>>(results, "heating.burner.statistics", "hours", "starts");
            await GetFeature<string>(results, "heating.circuits.0.operating.modes.active", "value");
            await GetFeature<string>(results, "heating.circuits.0.operating.programs.active", "value");
            await GetFeature<ClassNullable<bool>, ClassNullable<double>>(results, "heating.circuits.0.operating.programs.normal", "active", "temperature");
            await GetFeature<ClassNullable<bool>, ClassNullable<double>>(results, "heating.circuits.0.operating.programs.reduced", "active", "temperature");
            await GetFeature<ClassNullable<bool>, ClassNullable<double>>(results, "heating.circuits.0.operating.programs.comfort", "active", "temperature");
            await GetFeature<string, ClassNullable<double>>(results, "heating.circuits.0.sensors.temperature.supply", "status", "value");
            await GetFeature<string>(results, "heating.circuits.0.circulation.pump", "status");
            await GetFeature<string>(results, "heating.circuits.1.operating.modes.active", "value");
            await GetFeature<string>(results, "heating.circuits.1.operating.programs.active", "value");
            await GetFeature<ClassNullable<bool>, ClassNullable<double>>(results, "heating.circuits.1.operating.programs.normal", "active", "temperature");
            await GetFeature<ClassNullable<bool>, ClassNullable<double>>(results, "heating.circuits.1.operating.programs.reduced", "active", "temperature");
            await GetFeature<ClassNullable<bool>, ClassNullable<double>>(results, "heating.circuits.1.operating.programs.comfort", "active", "temperature");
            await GetFeature<string, ClassNullable<double>>(results, "heating.circuits.1.sensors.temperature.supply", "status", "value");
            await GetFeature<string>(results, "heating.circuits.1.circulation.pump", "status");
            await GetFeature<string, ClassNullable<double>>(results, "heating.dhw.sensors.temperature.hotWaterStorage", "status", "value");
            await GetFeature<string>(results, "heating.dhw.pumps.primary", "status");
            await GetFeature<string>(results, "heating.dhw.pumps.circulation", "status");
            await GetFeature<string, ClassNullable<double>>(results, "heating.boiler.sensors.temperature.main", "status", "value");
            await GetFeature<ClassNullable<double>>(results, "heating.burner.modulation", "value");

            return results;
        }

        private async Task GetFeature<T>(Dictionary<string, string> results, string featureName, string attrName) where T : class
        {
            var result = await ParseFeatureResponse<T>(await GetFeature(featureName), attrName);
            results.Add(featureName, result?.ToString() ?? "null");
        }

        private async Task GetFeature<T1, T2>(Dictionary<string, string> results, string featureName, params string[] attrNames) where T1 : class where T2 : class
        {
            var result = await ParseFeatureResponse<T1, T2>(await GetFeature(featureName), attrNames);
            results.Add(featureName, (result.Item1?.ToString() ?? "null") + " " + (result.Item2?.ToString() ?? "null"));
        }

        private async Task<HttpResponseMessage> GetFeature(string featureName)
        {
            await Authenticate();

            string baseUrl = "https://api.viessmann-platform.io/operational-data/v1/";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{baseUrl}installations/{_installationId}/gateways/{_gatewayId}/devices/0/features/{featureName}"),
                Method = HttpMethod.Get
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);

            return await _authClient.SendAsync(request);
        }

        private async Task Authenticate()
        {
            if (_tokenStore.IsAccessTokenValid())
                return;
            
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{_authUri}?type=web_server&client_id={_clientId}&redirect_uri={_redirectUri}&response_type=code"),
                Method = HttpMethod.Get,
            };

            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);

            var response = await _authClient.SendAsync(request);
            var location = response.Headers.Location.AbsoluteUri;

            var prefix = $"{_redirectUri}?code=";
            if (!location.StartsWith(prefix) || location.Length <= prefix.Length)
                throw new Exception("could not retrieve auth code");

            var authorization_code = location.Substring(prefix.Length);

            Tuple<string, DateTime> loadedToken = await ParseTokenResponse(await _client.PostAsync(
                new Uri(_tokenUri), new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("client_secret", _clientSecret),
                    new KeyValuePair<string, string>("code", authorization_code),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri)
                })));

            await _tokenStore.UpdateToken(loadedToken.Item1, loadedToken.Item2, string.Empty);
        }

        private async Task<Tuple<string, DateTime>> ParseTokenResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                access_token = "",
                expires_in = "",
                token_type = ""
            };
            var responseStr = await response.Content.ReadAsStringAsync();
            var authRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            return new Tuple<string, DateTime>(authRaw.access_token, DateTime.Now.AddSeconds(int.Parse(authRaw.expires_in)));
        }

        private class ClassNullable<T> where T : struct
        {
            private T? _val;
            public ClassNullable(T? value) { _val = value; }
            public override string ToString() { return _val?.ToString(); }
            public static implicit operator ClassNullable<T>(T? value) { return new ClassNullable<T>(value); }
            public static implicit operator T?(ClassNullable<T> nullable) { return nullable._val; }
        }

        private async Task<T> ParseFeatureResponse<T>(HttpResponseMessage response, string attrName) where T : class
        {
            return (await ParseFeatureResponse<T, string>(response, attrName)).Item1;
        }

        private async Task<Tuple<T1, T2>> ParseFeatureResponse<T1, T2>(HttpResponseMessage response, params string[] attrNames) where T1 : class where T2 : class
        {
            var subDefLinks = new[] { new
                {
                    rel = new List<string>(),
                    href = ""
                } };
            var subDefEntities = new[] { new
                {
                    rel = new List<string>(),
                    properties = new {
                        apiVersion = 0,
                        isEnabled = true,
                        isReady = true,
                        gatewayId = "",
                        feature = "",
                        uri = "",
                        deviceId = "",
                        timestamp = ""
                    }
                } };
            var definition = new
            {
                links = subDefLinks,
                @class = new List<string>(),
                properties = new
                {
                    value = new
                    {
                        type = "",
                        value = 0d
                    },
                    active = new
                    {
                        type = "",
                        value = false
                    },
                    status = new
                    {
                        type = "",
                        value = ""
                    },
                    temperature = new
                    {
                        type = "",
                        value = 0d
                    },
                    hours = new
                    {
                        type = "",
                        value = 0d
                    },
                    starts = new
                    {
                        type = "",
                        value = 0d
                    }
                },
                entities = subDefEntities,
                actions = new List<object>()
            };
            var definitionStrVal = new
            {
                links = subDefLinks,
                @class = new List<string>(),
                properties = new
                {
                    value = new
                    {
                        type = "",
                        value = ""
                    }
                },
                entities = subDefEntities,
                actions = new List<object>()
            };

            var responseStr = await response.Content.ReadAsStringAsync();

            var typeStringRaw = JsonConvert.DeserializeAnonymousType(responseStr, new { properties = new { value = new { type = "" } } });
            if (typeStringRaw.properties?.value?.type?.Trim()?.Equals("string", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                var featureStrRaw = JsonConvert.DeserializeAnonymousType(responseStr, definitionStrVal);
                return new Tuple<T1, T2>(featureStrRaw.properties?.value?.value as T1, default(T2));
            }

            var featureRaw = JsonConvert.DeserializeAnonymousType(responseStr, definition);
            switch (attrNames.FirstOrDefault()?.Trim()?.ToLowerInvariant() ?? string.Empty)
            {
                case "active":
                    return new Tuple<T1, T2>(((ClassNullable<bool>)featureRaw.properties?.active?.value) as T1, ((ClassNullable<double>)featureRaw.properties?.temperature?.value) as T2);
                case "hours":
                    return new Tuple<T1, T2>(((ClassNullable<double>)featureRaw.properties?.hours?.value) as T1, ((ClassNullable<double>)featureRaw.properties?.starts?.value) as T2);
                case "status":
                    return new Tuple<T1, T2>(featureRaw.properties?.status?.value as T1, ((ClassNullable<double>)featureRaw.properties?.value?.value) as T2);
                case "value":
                    return new Tuple<T1, T2>(((ClassNullable<double>)featureRaw.properties?.value?.value) as T1, default(T2));
                default:
                    return new Tuple<T1, T2>(default(T1), default(T2));
            }
        }
    }
}
