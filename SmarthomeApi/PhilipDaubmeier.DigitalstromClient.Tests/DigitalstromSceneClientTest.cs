using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
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
    public class DigitalstromSceneClientTest
    {
        private Zone zoneKitchen = 32027;

        [Fact]
        public async Task TestEventSubscription()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AutoFlush = true;

            var subscription = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/subscribe")
                                       .WithQueryString("token=5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2")
                                       .Respond("application/json", @"{ ""ok"": true }");

            var sceneClient = new DigitalstromSceneClient(mockHttp.AddAuthMock().ToMockProvider());

            await Task.Delay(100);
            mockHttp.Flush();
            
            Assert.Equal(2, mockHttp.GetMatchCount(subscription));
        }

        [Fact]
        public async Task TestSceneModel()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.AutoFlush = false;

            var sceneClient = new DigitalstromSceneClient(mockHttp.AddAuthMock().ToMockProvider());

            var events = new List<DssEvent>();
            sceneClient.ApiEventRaised += (s, e) => { events.Add(e.ApiEvent); };

            await Task.Delay(50);
            sceneClient.callScene(zoneKitchen, Group.Color.Yellow, Scene.SceneCommand.Preset1);
            await Task.Delay(50);

            Assert.True(sceneClient.Scenes[zoneKitchen, Group.Color.Yellow].Value == Scene.SceneCommand.Preset1);

            sceneClient.callScene(zoneKitchen, Group.Color.Yellow, Scene.SceneCommand.Preset2);
            await Task.Delay(50);

            Assert.True(sceneClient.Scenes[zoneKitchen, Group.Color.Yellow].Value == Scene.SceneCommand.Preset2);

            sceneClient.callScene(zoneKitchen, Group.Color.Yellow, Scene.SceneCommand.Preset1);
            await Task.Delay(50);

            Assert.NotEmpty(events.Where(e => e.systemEvent == SystemEventName.EventType.CallScene));
        }
    }
}