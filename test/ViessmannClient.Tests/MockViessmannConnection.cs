using PhilipDaubmeier.ViessmannClient.Network;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.ViessmannClient.Tests
{
    public static class MockViessmannConnection
    {
        private const string _authUri = "https://iam.viessmann.com/idp/v2/authorize";
        private const string _tokenUri = "https://iam.viessmann.com/idp/v2/token";
        private const string _redirectUri = "http://localhost:4000";

        private const string _clientId = "1234561unittestidf91d15ff4caceee";
        private const string _authorizationCode = "123_unittest_authorization_code_456";
        
        public static string BaseUri => "https://api.viessmann-platform.io/";
        public static string AppToken => "5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2";
        public static string RefreshToken => "9b22aceff_dummy_unittest_token_173cba864cbae45f34efa22bc47544111";
        public static string CodeChallenge => "SlvCf8TdHPDGMs-dummy-nHjgX-nRICB-fRtQ0Uhy7g";

        public static long InstallationId => 12345;
        public static long GatewayId => 3344556677;
        public static long DeviceId => 0;

        public class ViessmannMockConnectionProvider<T> : ViessmannConnectionProvider<T>
        {
            public ViessmannMockConnectionProvider(IViessmannAuth authData, HttpClient mockClient)
                : base(authData)
            {
                Client = mockClient;
                ClientId = _clientId;
                RedirectUri = _redirectUri;
            }
        }

        private static readonly IViessmannAuth auth = new ViessmannAuth();

        public static ViessmannConnectionProvider<ViessmannPlatformClient> ToMockProvider(this MockHttpMessageHandler mockHandler)
        {
            auth.UpdateTokenAsync(null, DateTime.MinValue, RefreshToken);
            return new ViessmannMockConnectionProvider<ViessmannPlatformClient>(auth, new HttpClient(mockHandler));
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{_authUri}")
                    .WithExactQueryString($"client_id={_clientId}&redirect_uri={_redirectUri}&response_type=code&code_challenge={CodeChallenge}&code_challenge_method=S256&response_type=code&scope=IoT%20User%20offline_access")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_redirectUri}?code={_authorizationCode}")
                    }, "text/plain", "");

            mockHttp.When($"{_tokenUri}")
                    .Respond("application/json", @"{
                                  ""access_token"": """ + AppToken + @""",
                                  ""expires_in"": 36000,
                                  ""token_type"": ""Bearer"",
                                  ""refresh_token"": """ + RefreshToken + @"""
                              }");

            return mockHttp;
        }
    }
}