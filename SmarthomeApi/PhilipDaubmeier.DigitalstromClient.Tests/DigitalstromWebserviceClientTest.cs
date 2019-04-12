using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Network;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Tests
{
    public class DigitalstromWebserviceClientTest
    {
        private static IDigitalstromAuth auth = new EphemeralDigitalstromAuth("DigitalstromClientUnittests", "***REMOVED***", "mocksecret");
        private static string baseUri = "https://unittestdummy0000123456789abcdef.digitalstrom.net:8080";
        private static UriPriorityList uris = new UriPriorityList(new List<Uri>() { new Uri(baseUri) });
        private static DigitalstromConnectionProvider WithMock(HttpMessageHandler mock) => new DigitalstromConnectionProvider(uris, auth, null, mock);

        private static void AddAuthMock(MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{baseUri}/json/system/requestApplicationToken")
                    .WithExactQueryString("applicationName=DigitalstromClientUnittests")
                    .Respond("application/json", @"{
                                  ""result"": {
                                      ""applicationToken"": ""3b00f95e7_dummy_unittest_token_8e2adbcabcd8b67391e3fd0a95f68d40e""
                                  },
                                  ""ok"": true
                              }");

            mockHttp.When($"{baseUri}/json/system/login")
                    .WithExactQueryString("user=***REMOVED***&password=mocksecret")
                    .Respond("application/json", @"{
                                  ""result"": {
                                      ""token"": ""eb7a72928_dummy_unittest_token_6bd708977a2beb50278765af04c839217""
                                  },
                                  ""ok"": true
                              }");

            mockHttp.When($"{baseUri}/json/system/enableToken")
                    .WithExactQueryString("applicationToken=3b00f95e7_dummy_unittest_token_8e2adbcabcd8b67391e3fd0a95f68d40e&token=eb7a72928_dummy_unittest_token_6bd708977a2beb50278765af04c839217")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");

            mockHttp.When($"{baseUri}/json/system/loginApplication")
                    .WithExactQueryString("loginToken=3b00f95e7_dummy_unittest_token_8e2adbcabcd8b67391e3fd0a95f68d40e")
                    .Respond("application/json", @"{
                                  ""result"": {
                                      ""token"": ""5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2""
                                  },
                                  ""ok"": true
                              }");
        }

        [Fact]
        public async Task TestGetZonesAndLastCalledScenes()
        {
            var mockHttp = new MockHttpMessageHandler();
            AddAuthMock(mockHttp);

            mockHttp.When($"{baseUri}/json/property/query")
                    .WithExactQueryString("query=/apartment/zones/*(ZoneID)/groups/*(group,lastCalledScene)&token=5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"":
                                      [
                                          {
                                              ""ZoneID"": 4,
                                              ""groups"":
                                              [
                                                  { ""group"": 0, ""lastCalledScene"": 0 },
                                                  { ""group"": 1, ""lastCalledScene"": 0 },
                                                  { ""group"": 2, ""lastCalledScene"": 0 },
                                                  { ""group"": 3, ""lastCalledScene"": 0 },
                                                  { ""group"": 4, ""lastCalledScene"": 5 },
                                                  { ""group"": 5, ""lastCalledScene"": 0 },
                                                  { ""group"": 6, ""lastCalledScene"": 0 },
                                                  { ""group"": 7, ""lastCalledScene"": 0 },
                                                  { ""group"": 8, ""lastCalledScene"": 40 },
                                                  { ""group"": 9, ""lastCalledScene"": 0 },
                                                  { ""group"": 10, ""lastCalledScene"": 0 },
                                                  { ""group"": 11, ""lastCalledScene"": 0 },
                                                  { ""group"": 12, ""lastCalledScene"": 0 },
                                                  { ""group"": 48, ""lastCalledScene"": 0 }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromWebserviceClient(WithMock(mockHttp));
            
            var zonesScenes = await dsApiClient.GetZonesAndLastCalledScenes();
            Assert.NotEmpty(zonesScenes.zones);

            Assert.Equal(5, zonesScenes.zones[0].groups.First(g => g.group == 4).lastCalledScene);
            Assert.Equal(40, zonesScenes.zones[0].groups.First(g => g.group == 8).lastCalledScene);
        }
    }
}