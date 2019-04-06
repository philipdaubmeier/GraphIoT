using DigitalstromClient.Model;
using DigitalstromClient.Model.Core;
using DigitalstromClient.Model.Events;
using DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DigitalstromClient
{
    public class DigitalstromSceneClientTest
    {
        private static IDigitalstromAuth digitalstromAuthData = new EphemeralDigitalstromAuth("DigitalstromClientUnittests")
        {
            Username = "***REMOVED***",
            UserPassword = "***REMOVED***"
        };

        private static Uri uri1 = new Uri("***REMOVED***");
        private static Uri uri2 = new Uri("https://***REMOVED***.digitalstrom.net:8080/");

        private Zone zoneKitchen = 32027;

        [Fact]
        public async Task Test()
        {
            var dsApiClient = new DigitalstromWebserviceClient(uri1, uri2, digitalstromAuthData);
            var sceneClient = new DigitalstromSceneClient(dsApiClient);

            var events = new List<DssEvent>();
            sceneClient.ApiEventRaised += (s, e) => { events.Add(e.ApiEvent); };

            await Task.Delay(1000);
            sceneClient.callScene(zoneKitchen, Group.Color.Yellow, Scene.SceneCommand.Preset1);
            await Task.Delay(2000);

            Assert.True(sceneClient.Scenes[zoneKitchen, Group.Color.Yellow].Value == Scene.SceneCommand.Preset1);

            sceneClient.callScene(zoneKitchen, Group.Color.Yellow, Scene.SceneCommand.Preset2);
            await Task.Delay(2000);

            Assert.True(sceneClient.Scenes[zoneKitchen, Group.Color.Yellow].Value == Scene.SceneCommand.Preset2);

            sceneClient.callScene(zoneKitchen, Group.Color.Yellow, Scene.SceneCommand.Preset1);
            await Task.Delay(1000);

            Assert.NotEmpty(events.Where(e => e.systemEvent == SystemEventName.EventType.CallScene));
        }
    }
}