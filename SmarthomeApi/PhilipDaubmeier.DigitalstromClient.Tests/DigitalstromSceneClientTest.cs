using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Network;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var subscription = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/subscribe")
                                       .WithQueryString($"token={MockDigitalstromConnection.AppToken}")
                                       .Respond("application/json", @"{ ""ok"": true }");

            using (var sceneClient = new DigitalstromSceneClient(mockHttp.AddAuthMock().ToMockProvider()))
            {
                // Wait for the threads to start and make the subscriptions
                await Task.Delay(100);
            }
            
            Assert.Equal(2, mockHttp.GetMatchCount(subscription));
        }

        [Fact]
        public async Task TestSceneModel()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/subscribe")
                    .WithQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{ ""ok"": true }");

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/zone/callScene")
                    .WithExactQueryString($"id=32027&groupID=1&sceneNumber=5&force=true&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/zones/*(ZoneID)/groups/*(group,lastCalledScene)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"":
                                      [
                                          {
                                              ""ZoneID"": 32027,
                                              ""groups"":
                                              [
                                                  { ""group"": 4, ""lastCalledScene"": 5 },
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/zones/*(ZoneID)/groups/group0/sensor/*(type,value,time)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"": [
                                          {
                                              ""ZoneID"": 32027,
                                              ""sensor"": [
                                                  {
                                                      ""type"": 9,
                                                      ""value"": 21.3,
                                                      ""time"": 1556789036
                                                  }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            Func<Scene, string> MockedSceneEvent = scene => @"{
                                  ""result"":
                                  {
                                      ""events"": [
                                          {
                                              ""name"": ""callScene"",
                                              ""properties"": {
                                                  ""callOrigin"": ""2"",
                                                  ""sceneID"": """ + ((int)scene).ToString() + @""",
                                                  ""forced"": ""true"",
                                                  ""groupID"": ""1"",
                                                  ""zoneID"": ""32027"",
                                                  ""originToken"": ""5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2"",
                                                  ""originDSUID"": ""0000000000000000000000000000000000""
                                              },
                                              ""source"": {
                                                  ""set"": "".zone(32027).group(1)"",
                                                  ""groupID"": 1,
                                                  ""zoneID"": 32027,
                                                  ""isApartment"": false,
                                                  ""isGroup"": true,
                                                  ""isDevice"": false
                                              }
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }";

            var mockedEventResponse = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                    .WithExactQueryString($"subscriptionID=42&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", MockedSceneEvent(Scene.SceneCommand.Preset0));

            using (var sceneClient = new DigitalstromSceneClient(mockHttp.AddAuthMock().ToMockProvider()))
            {
                await Task.Delay(100);
                mockHttp.AutoFlush = true;
                mockHttp.Flush();

                var events = new List<DssEvent>();
                var errors = new List<Exception>();
                sceneClient.ApiEventRaised += (s, e) => { events.Add(e.ApiEvent); };
                sceneClient.ErrorOccured += (s, e) => { errors.Add(e.Error); };

                mockedEventResponse.Respond("application/json", MockedSceneEvent(Scene.SceneCommand.Preset1));
                mockHttp.Flush();
                await Task.Delay(50);

                Assert.Equal((int)Scene.SceneCommand.Preset1, (int)sceneClient.Scenes[zoneKitchen, Group.Color.Yellow].Value);

                mockedEventResponse.Respond("application/json", MockedSceneEvent(Scene.SceneCommand.Preset2));
                mockHttp.Flush();
                await Task.Delay(50);

                Assert.Equal((int)Scene.SceneCommand.Preset2, (int)sceneClient.Scenes[zoneKitchen, Group.Color.Yellow].Value);

                mockedEventResponse.Respond("application/json", MockedSceneEvent(Scene.SceneCommand.Preset1));
                mockHttp.Flush();
                await Task.Delay(50);

                Assert.Equal((int)Scene.SceneCommand.Preset1, (int)sceneClient.Scenes[zoneKitchen, Group.Color.Yellow].Value);

                Assert.NotEmpty(events.Where(e => e.systemEvent == SystemEventName.EventType.CallScene));
                Assert.Empty(errors);
            }
        }
    }
}