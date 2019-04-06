using DigitalstromClient.Model;
using DigitalstromClient.Network;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DigitalstromClient
{
    public class DigitalstromWebserviceClientTest
    {
        private static IDigitalstromAuth digitalstromAuthData = new EphemeralDigitalstromAuth("DigitalstromClientUnittests")
        {
            Username = "***REMOVED***",
            UserPassword = "***REMOVED***"
        };

        private static Uri uri1 = new Uri("***REMOVED***");
        private static Uri uri2 = new Uri("https://***REMOVED***.digitalstrom.net:8080/");
        
        [Fact]
        public async Task TestGetZonesAndLastCalledScenes()
        {
            var dsApiClient = new DigitalstromWebserviceClient(uri1, uri2, digitalstromAuthData);

            var zonesScenes = await dsApiClient.GetZonesAndLastCalledScenes();
            Assert.NotEmpty(zonesScenes.zones);
        }
    }
}