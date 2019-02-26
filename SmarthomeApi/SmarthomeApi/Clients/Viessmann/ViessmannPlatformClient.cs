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
            _tokenStore = new TokenStore(databaseContext, "viessmann");
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
    }
}
