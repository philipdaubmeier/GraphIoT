using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Netatmo
{
    internal class NetatmoWebserviceClient
    {
        private Uri _baseUri;
        private NetatmoAuth _authData;

        /// <summary>
        /// Connects to the netatmo REST webservice at the given uri with the given
        /// app and user credentials given via the authentication data object. If
        /// a valid unexpired access token is given in the auth data, it is used directly.
        /// </summary>
        /// <param name="baseUri">The uri of the netatmo RESTful webservice</param>
        /// <param name="authData">The authentication information needed to use for
        /// the webservice or to perform a new or renewed authentication</param>
        public NetatmoWebserviceClient(Uri baseUri, NetatmoAuth authData)
        {
            _baseUri = baseUri;
            _authData = authData;
        }

        public async Task<Tuple<string, string>> GetCameraPicture()
        {
            await Authenticate();
            return await LoadCameraPicture();
        }

        private async Task Authenticate()
        {
            if (_authData.MustAuthenticate())
                await GetInitialAccessToken();
            else if (_authData.MustRefreshToken())
                await RefreshAccessToken();
        }

        private async Task GetInitialAccessToken()
        {
            var client = new HttpClient();
            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", _authData.NetatmoAppId),
                new KeyValuePair<string, string>("client_secret", _authData.NetatmoAppSecret),
                new KeyValuePair<string, string>("username", _authData.Username),
                new KeyValuePair<string, string>("password", _authData.UserPassword),
                new KeyValuePair<string, string>("scope", _authData.Scope)
            });
            await ParseTokenResponse(await client.PostAsync(new Uri(_baseUri, "/oauth2/token"), postData));
        }

        private async Task RefreshAccessToken()
        {
            var client = new HttpClient();
            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", _authData.RefreshToken),
                new KeyValuePair<string, string>("client_id", _authData.NetatmoAppId),
                new KeyValuePair<string, string>("client_secret", _authData.NetatmoAppSecret),
            });
            await ParseTokenResponse(await client.PostAsync(new Uri(_baseUri, "/oauth2/token"), postData));
        }

        private async Task ParseTokenResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                access_token = "",
                expires_in = 0d,
                refresh_token = ""
            };
            var authRaw = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);

            _authData.AccessToken = authRaw.access_token;
            _authData.Expiration = DateTime.UtcNow.AddSeconds(authRaw.expires_in);
            _authData.RefreshToken = authRaw.refresh_token;
        }

        private async Task<Tuple<string, string>> LoadCameraPicture()
        {
            var client = new HttpClient();
            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("access_token", _authData.AccessToken),
                new KeyValuePair<string, string>("size", "1")
            });
            return await ParseHomeDataResponse(await client.PostAsync(new Uri(_baseUri, "/api/gethomedata"), postData));
        }

        private async Task<Tuple<string, string>> ParseHomeDataResponse(HttpResponseMessage response)
        {
            var definition = new
            {
                body = new
                {
                    homes = new[] { new
                    {
                        id = "",
                        name = "",
                        place = new
                        {
                            city = "",
                            country = "",
                            timezone = ""
                        },
                        cameras = new[] { new
                        {
                            id = "",
                            type = "",
                            status = "",
                            vpn_url = "",
                            is_local = true,
                            sd_status = "",
                            alim_status = "",
                            name = "",
                            last_setup = 0,
                            light_mode_status = ""
                        } },
                        events = new[] { new
                        {
                            id = "",
                            type = "",
                            time = 0,
                            camera_id = "",
                            device_id = "",
                            video_id = "",
                            video_status = "",
                            event_list = new[] { new
                            {
                                type = "",
                                time = 0,
                                offset = 0,
                                id = "",
                                message = "",
                                snapshot = new
                                {
                                    id = "",
                                    version = 0,
                                    key = "",
                                    url = "",
                                    filename = ""
                                },
                                vignette = new
                                {
                                    id = "",
                                    version = 0,
                                    key = "",
                                    url = "",
                                    filename = ""
                                }
                            } }
                        } }
                    } },
                    user = new
                    {
                        reg_locale = "",
                        lang = "",
                        country = "",
                        mail = ""
                    },
                    global_info = new
                    {
                        show_tags = true
                    }
                },
                status = "",
                time_exec = 0d,
                time_server = 0
            };
            var rawData = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), definition);
            
            var home = rawData.body.homes.First(x => x.name.Equals("Karlskron", StringComparison.InvariantCultureIgnoreCase));
            var cameraId = home.cameras.First(x => x.name.Equals("Phils Presence", StringComparison.InvariantCultureIgnoreCase)).id;
            var events = home.events.First(x => x.camera_id == cameraId).event_list.Where(x => !string.IsNullOrWhiteSpace(x.snapshot.id) && !string.IsNullOrWhiteSpace(x.snapshot.key));
            var newestSnapshot = events.OrderByDescending(x => x.time).First().snapshot;
            return new Tuple<string, string>(newestSnapshot.id, newestSnapshot.key);
        }
    }
}
