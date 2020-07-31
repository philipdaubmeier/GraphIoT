using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Network;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromDssMock
{
    public static class MockDigitalstromConnection
    {
        public class TestGenerationHttpMessageHandler : MockHttpMessageHandler
        {
            public string? lastCalledUri = null;
            public string LastCalledUri => lastCalledUri ?? string.Empty;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                lastCalledUri = request.RequestUri?.ToString();

                return base.SendAsync(request, cancellationToken);
            }
        }

        private static readonly IDigitalstromAuth auth = new EphemeralDigitalstromAuth("DigitalstromClientUnittests", "dssadmin", "mocksecret");
        private static readonly UriPriorityList testGenerationUris = new UriPriorityList(new List<Uri>() { new Uri("https://uri") }, new List<bool>() { true });
        private static readonly UriPriorityList mockUris = new UriPriorityList(new List<Uri>() { new Uri(BaseUri) });

        public static string BaseUri => "https://unittestdummy0000123456789abcdef.digitalstrom.net:8080";
        public static string AppToken => "5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2";

        public static DigitalstromConnectionProvider ToTestGenerationProvider(this MockHttpMessageHandler mockHandler)
        {
            if (!(mockHandler is TestGenerationHttpMessageHandler))
                throw new ArgumentException("TestGenerationHttpMessageHandler type expected", nameof(mockHandler));

            return new DigitalstromConnectionProvider(testGenerationUris, auth, null, null, mockHandler);
        }

        public static DigitalstromConnectionProvider ToMockProvider(this MockHttpMessageHandler mockHandler)
        {
            return new DigitalstromConnectionProvider(mockUris, auth, null, null, mockHandler);
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{BaseUri}/json/system/requestApplicationToken")
                    .WithExactQueryString("applicationName=DigitalstromClientUnittests")
                    .Respond("application/json", @"{
                                  ""result"": {
                                      ""applicationToken"": ""3b00f95e7_dummy_unittest_token_8e2adbcabcd8b67391e3fd0a95f68d40e""
                                  },
                                  ""ok"": true
                              }");

            mockHttp.When($"{BaseUri}/json/system/login")
                    .WithExactQueryString("user=dssadmin&password=mocksecret")
                    .Respond("application/json", @"{
                                  ""result"": {
                                      ""token"": ""eb7a72928_dummy_unittest_token_6bd708977a2beb50278765af04c839217""
                                  },
                                  ""ok"": true
                              }");

            mockHttp.When($"{BaseUri}/json/system/enableToken")
                    .WithExactQueryString("applicationToken=3b00f95e7_dummy_unittest_token_8e2adbcabcd8b67391e3fd0a95f68d40e&token=eb7a72928_dummy_unittest_token_6bd708977a2beb50278765af04c839217")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");

            mockHttp.When($"{BaseUri}/json/system/loginApplication")
                    .WithExactQueryString("loginToken=3b00f95e7_dummy_unittest_token_8e2adbcabcd8b67391e3fd0a95f68d40e")
                    .Respond("application/json", @"{
                                  ""result"": {
                                      ""token"": """ + AppToken + @"""
                                  },
                                  ""ok"": true
                              }");

            return mockHttp;
        }
    }
}