using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Tests;
using PhilipDaubmeier.DigitalstromClient.Twin;
using RichardSzalay.MockHttp;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Twin.Tests
{
    public static class TwinMockExtensions
    {
        public static async Task MockDssEventAsync(this MockHttpMessageHandler mockHttp, DssEventSubscriber eventSubscriber, MockedRequest mockedEventResponse, string responseContent, int timeoutMillis = 1000)
        {
            DateTime start = DateTime.UtcNow;
            bool eventReceived = false;
            eventSubscriber.ApiEventRaised += (s, e) => eventReceived = true;

            // set the response and flush it - this will create a response in the long polling request in the event subscriber
            mockedEventResponse.Respond("application/json", responseContent);
            mockHttp.Flush();

            // give the event polling thread a chance to receive and handle the event, it will set the eventReceived
            while (!eventReceived && (DateTime.UtcNow - start).TotalMilliseconds < timeoutMillis)
                await Task.Delay(1);
        }

        public static void AddInitialAndSubscribeMocks(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/subscribe")
                    .WithQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{ ""ok"": true }");

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
        }

        public static string ToMockedSceneEvent(this SceneCommand scene)
        {
            return ((Scene)scene).ToMockedSceneEvent();
        }

        public static string ToMockedSceneEvent(this Scene scene)
        {
            return @"{
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
        }
    }
}
