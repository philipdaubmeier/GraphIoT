using PhilipDaubmeier.DigitalstromClient.Network;
using RichardSzalay.MockHttp;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Tests
{
    public class DigitalstromWebserviceClientTest
    {
        [Fact]
        public async Task TestGetZonesAndLastCalledScenes()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());
            
            var zonesScenes = await dsApiClient.GetZonesAndLastCalledScenes();
            Assert.NotEmpty(zonesScenes.zones);

            Assert.Equal(5, zonesScenes.zones[0].groups.First(g => g.group == 4).lastCalledScene);
            Assert.Equal(40, zonesScenes.zones[0].groups.First(g => g.group == 8).lastCalledScene);
        }
    }
}