using PhilipDaubmeier.NetatmoClient.Network;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace PhilipDaubmeier.NetatmoClient.Tests
{
    public static class MockNetatmoConnection
    {
        private const string _clientId = "1234561unittestidf91d15ff4caceee";
        private const string _clientSecret = "9876561unittestsecret15ff4caceff";
        private const string _scope = "read_station read_presence access_presence";

        private const string _username = "john@doe.com";
        private const string _password = "secretpassword";

        public static string BaseUri => "https://api.netatmo.net";
        public static string AppToken => "5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2";
        public static string RefreshToken => "716289babc_dummy_unittest_refresh_token_83025a07162890587bea58987be";

        private static readonly INetatmoAuth auth = new NetatmoAuth(_username, _password);

        public class NetatmoMockConnectionProvider : NetatmoConnectionProvider
        {
            public NetatmoMockConnectionProvider(INetatmoAuth authData, HttpClient mockClient)
                : base(authData)
            {
                AppId = _clientId;
                AppSecret = _clientSecret;
                Scope = _scope;
                Client = mockClient;
            }
        }

        public static NetatmoConnectionProvider ToMockProvider(this MockHttpMessageHandler mockHandler)
        {
            return new NetatmoMockConnectionProvider(auth, mockHandler.ToHttpClient());
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{BaseUri}/oauth2/token")
                    .WithFormData(new[]
                    {
                        ("grant_type", "password"),
                        ("client_id", _clientId),
                        ("client_secret", _clientSecret),
                        ("username", _username),
                        ("password", _password),
                        ("scope", _scope)
                    }.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
                    .Respond("application/json",
                    @"{
                        ""access_token"": """ + AppToken + @""",
                        ""expires_in"": 36000,
                        ""refresh_token"": """ + RefreshToken + @""",
                    }");

            mockHttp.When($"{BaseUri}/oauth2/token")
                    .WithFormData(new[]
                    {
                        ("grant_type", "refresh_token"),
                        ("refresh_token", RefreshToken),
                        ("client_id", _clientId),
                        ("client_secret", _clientSecret)
                    }.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
                    .Respond("application/json",
                    @"{
                        ""access_token"": """ + AppToken + @""",
                        ""expires_in"": 36000,
                        ""refresh_token"": """ + RefreshToken + @""",
                    }");

            return mockHttp;
        }
    }
}