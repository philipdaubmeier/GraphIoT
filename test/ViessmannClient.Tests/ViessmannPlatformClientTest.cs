using RichardSzalay.MockHttp;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.ViessmannClient.Tests
{
    public class ViessmannPlatformClientTest
    {
        [Fact]
        public async Task TestGetTemperatureControlStatus()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}iot/v1/equipment/installations")
                    .Respond("application/json", @"{
                                      ""data"": [
                                          {
                                              ""id"": " + MockViessmannConnection.InstallationId + @",
                                              ""description"": null,
                                              ""address"": {
                                                  ""street"": ""Long test street"",
                                                  ""houseNumber"": ""1001"",
                                                  ""zip"": ""12345"",
                                                  ""city"": ""Unittest Town"",
                                                  ""region"": null,
                                                  ""country"": ""de"",
                                                  ""phoneNumber"": null,
                                                  ""faxNumber"": null,
                                                  ""geolocation"": {
                                                      ""latitude"": 37.377166,
                                                      ""longitude"": -122.086966,
                                                      ""timeZone"": ""Europe/Berlin""
                                                  }
                                              },
                                              ""registeredAt"": ""2017-01-29T18:19:44.000Z"",
                                              ""updatedAt"": ""2018-06-09T17:57:43.636Z"",
                                              ""aggregatedStatus"": ""WorksProperly"",
                                              ""servicedBy"": null,
                                              ""heatingType"": null
                                          }
                                      ],
                                      ""cursor"": {
                                          ""next"": """"
                                      }
                                  }
                              }");

            using var viessmannClient = new ViessmannPlatformClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await viessmannClient.GetInstallations();

            Assert.Equal(MockViessmannConnection.InstallationId, result.First().Id.ToString());
        }
    }
}