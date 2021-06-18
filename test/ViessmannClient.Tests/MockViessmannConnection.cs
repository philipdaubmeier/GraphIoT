using PhilipDaubmeier.ViessmannClient.Network;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.ViessmannClient.Tests
{
    public static class MockViessmannConnection
    {
        private const string _authUri = "https://iam.viessmann.com/idp/v1/authorize";
        private const string _tokenUri = "https://iam.viessmann.com/idp/v1/token";
        private const string _redirectUri = "vicare://oauth-callback/everest";

        private const string _clientId = "1234561unittestidf91d15ff4caceee";
        private const string _clientSecret = "9876543unittestsecret093c7c08fff";
        private const string _authorizationCode = "123_unittest_authorization_code_456";

        public static string BaseUri => "https://api.viessmann-platform.io/";
        public static string AppToken => "5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2";

        public static long InstallationId => 12345;
        public static long GatewayId => 3344556677;
        public static long DeviceId => 0;

        public class ViessmannMockConnectionProvider<T> : ViessmannConnectionProvider<T>
        {
            public ViessmannMockConnectionProvider(IViessmannAuth authData, HttpClient mockClient, HttpClient mockAuthClient)
                : base(authData)
            {
                Client = mockClient;
                AuthClient = mockAuthClient;
                ClientId = _clientId;
                RedirectUri = _clientSecret;
            }
        }

        private static readonly IViessmannAuth auth = new ViessmannAuth("john@doe.com", "secretpassword");

        public static ViessmannConnectionProvider<ViessmannPlatformClient> ToMockProvider(this MockHttpMessageHandler mockHandler)
        {
            return new ViessmannMockConnectionProvider<ViessmannPlatformClient>(auth, new HttpClient(mockHandler), new HttpClient(mockHandler));
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{_authUri}")
                    .WithExactQueryString($"type=web_server&client_id={_clientId}&redirect_uri={_redirectUri}&response_type=code")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("Location", $"{_redirectUri}?code={_authorizationCode}")
                    }, "text/plain", "");

            mockHttp.When($"{_tokenUri}")
                    .Respond("application/json", @"{
                                  ""access_token"": """ + AppToken + @""",
                                  ""expires_in"": 36000,
                                  ""token_type"": ""Bearer""
                              }");

            return mockHttp;
        }
    }
}