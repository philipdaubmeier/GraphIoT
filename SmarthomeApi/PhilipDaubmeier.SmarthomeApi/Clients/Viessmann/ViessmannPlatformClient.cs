using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PhilipDaubmeier.SmarthomeApi.Database.Model;
using PhilipDaubmeier.SmarthomeApi.Model.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Clients.Viessmann
{
    public class ViessmannPlatformClient
    {
        private readonly IOptions<ViessmannConfig> _config;

        private static readonly HttpClientHandler _authClientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
        private static readonly HttpClient _authClient = new HttpClient(_authClientHandler);
        private static readonly HttpClient _client = new HttpClient();

        private const string _authUri = "https://iam.viessmann.com/idp/v1/authorize";
        private const string _tokenUri = "https://iam.viessmann.com/idp/v1/token";
        private const string _redirectUri = "vicare://oauth-callback/everest";

        private TokenStore<ViessmannPlatformClient> _tokenStore;

        public enum Circuit
        {
            Circuit0,
            Circuit1
        }

        public ViessmannPlatformClient(TokenStore<ViessmannPlatformClient> tokenStore, IOptions<ViessmannConfig> config)
        {
            _tokenStore = tokenStore;
            _config = config;
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

        public async Task<Tuple<string, double>> GetOutsideTemperature()
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature("heating.sensors.temperature.outside"), "status", "value");
            return new Tuple<string, double>(res.Item1, ((double?)res.Item2) ?? 0d);
        }

        public async Task<double> GetBoilerTemperature()
        {
            var res = await ParseFeatureResponse<ClassNullable<double>>(await GetFeature("heating.boiler.temperature"), "value");
            return ((double?)res) ?? 0d;
        }

        public async Task<bool> GetBurnerActiveStatus()
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>>(await GetFeature("heating.burner"), "active");
            return ((bool?)res) ?? false;
        }

        public async Task<Tuple<double, int>> GetBurnerStatistics()
        {
            var res = await ParseFeatureResponse<ClassNullable<double>, ClassNullable<double>>(await GetFeature("heating.burner.statistics"), "hours", "starts");
            return new Tuple<double, int>(((double?)res.Item1) ?? 0d, (int)(((double?)res.Item2) ?? 0d));
        }

        public async Task<string> GetCircuitOperatingMode(Circuit circuit)
        {
            return await ParseFeatureResponse<string>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.modes.active"), "value");
        }

        public async Task<string> GetCircuitActiveProgram(Circuit circuit)
        {
            return await ParseFeatureResponse<string>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.active"), "value");
        }

        public async Task<Tuple<bool, double>> GetCircuitProgramNormal(Circuit circuit)
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.normal"), "active", "temperature");
            return new Tuple<bool, double>(((bool?)res.Item1) ?? false, ((double?)res.Item2) ?? 0d);
        }

        public async Task<Tuple<bool, double>> GetCircuitProgramReduced(Circuit circuit)
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.reduced"), "active", "temperature");
            return new Tuple<bool, double>(((bool?)res.Item1) ?? false, ((double?)res.Item2) ?? 0d);
        }

        public async Task<Tuple<bool, double>> GetCircuitProgramComfort(Circuit circuit)
        {
            var res = await ParseFeatureResponse<ClassNullable<bool>, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.operating.programs.comfort"), "active", "temperature");
            return new Tuple<bool, double>(((bool?)res.Item1) ?? false, ((double?)res.Item2) ?? 0d);
        }

        public async Task<Tuple<string, double>> GetCircuitTemperature(Circuit circuit)
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.sensors.temperature.supply"), "status", "value");
            return new Tuple<string, double>(res.Item1, ((double?)res.Item2) ?? 0d);
        }

        public async Task<bool> GetCircuitCirculationPump(Circuit circuit)
        {
            return (await ParseFeatureResponse<string>(await GetFeature($"heating.circuits.{CircuitNumber(circuit)}.circulation.pump"), "status"))?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task<Tuple<string, double>> GetDhwStorageTemperature()
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature("heating.dhw.sensors.temperature.hotWaterStorage"), "status", "value");
            return new Tuple<string, double>(res.Item1, ((double?)res.Item2) ?? 0d);
        }

        public async Task<bool> GetDhwPrimaryPump()
        {
            return (await ParseFeatureResponse<string>(await GetFeature("heating.dhw.pumps.primary"), "status"))?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task<bool> GetDhwCirculationPump()
        {
            return (await ParseFeatureResponse<string>(await GetFeature("heating.dhw.pumps.circulation"), "status"))?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task<Tuple<string, double>> GetBoilerTemperatureMain()
        {
            var res = await ParseFeatureResponse<string, ClassNullable<double>>(await GetFeature("heating.boiler.sensors.temperature.main"), "status", "value");
            return new Tuple<string, double>(res.Item1, ((double?)res.Item2) ?? 0d);
        }

        public async Task<int> GetBurnerModulation()
        {
            var res = await ParseFeatureResponse<ClassNullable<double>>(await GetFeature("heating.burner.modulation"), "value");
            return (int)(((double?)res) ?? 0d);
        }

        private string CircuitNumber(Circuit circuit)
        {
            return circuit == Circuit.Circuit0 ? "0" : "1";
        }

        private async Task<HttpResponseMessage> GetFeature(string featureName)
        {
            await Authenticate();

            string baseUrl = "https://api.viessmann-platform.io/operational-data/v1/";
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{baseUrl}installations/{_config.Value.PlattformInstallationId}/gateways/{_config.Value.PlattformGatewayId}/devices/0/features/{featureName}"),
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
                RequestUri = new Uri($"{_authUri}?type=web_server&client_id={_config.Value.PlattformApiClientId}&redirect_uri={_redirectUri}&response_type=code"),
                Method = HttpMethod.Get,
            };

            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config.Value.Username}:{_config.Value.Password}"));
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
                    new KeyValuePair<string, string>("client_id", _config.Value.PlattformApiClientId),
                    new KeyValuePair<string, string>("client_secret", _config.Value.PlattformApiClientSecret),
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
