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

        private Zone zoneKitchen = 32027;

        [Fact]
        public async Task TestEventSubscription()
        {
            var mockHttp = new MockHttpMessageHandler();
            AddAuthMock(mockHttp);
            mockHttp.AutoFlush = true;

            var subscription = mockHttp.When($"{baseUri}/json/event/subscribe")
                                       .WithQueryString("token=5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2")
                                       .Respond("application/json", @"{ ""ok"": true }");

            var sceneClient = new DigitalstromSceneClient(WithMock(mockHttp));

            await Task.Delay(100);
            mockHttp.Flush();
            
            Assert.Equal(2, mockHttp.GetMatchCount(subscription));
        }

        [Fact]
        public async Task TestSceneModel()
        {
            var mockHttp = new MockHttpMessageHandler();
            AddAuthMock(mockHttp);
            mockHttp.AutoFlush = false;
            
            var sceneClient = new DigitalstromSceneClient(WithMock(mockHttp));
            
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