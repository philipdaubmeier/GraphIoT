using PhilipDaubmeier.DigitalstromClient.Model.Core;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromDssMock
{
    public static class DigitalstromDssMockExtensions
    {
        public static Zone ZoneIdKitchen => 32027;

        public static MockHttpMessageHandler AddSensorMocks(this MockHttpMessageHandler mockHttp, int resolution = 60 * 5, int valueCount = 1)
        {
            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/apartment/getSensorValues")
                    .WithExactQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""weather"": {
                                          ""WeatherIconId"": ""01n"",
                                          ""WeatherConditionId"": ""800"",
                                          ""WeatherServiceId"": ""7"",
                                          ""WeatherServiceTime"": ""2019-05-01T21:15:07.560Z""
                                      },
                                      ""outdoor"": {},
                                      ""zones"": [
                                          {
                                              ""id"": 65534,
                                              ""name"": """",
                                              ""values"": []
                                          },
                                          {
                                              ""id"": " + (int)ZoneIdKitchen + @",
                                              ""name"": ""Kitchen"",
                                              ""values"": [" + string.Join(',', Enumerable.Range(0, valueCount)
                                                  .Select(x => new Tuple<int, string>(x, DateTime.UtcNow.AddSeconds(-1 * resolution * (valueCount - x)).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture)))
                                                  .Select(x => @"
                                                  {
                                                      ""TemperatureValue"": " + (20.05M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""TemperatureValueTime"": """ + x.Item2 + @"""
                                                  },
                                                  {
                                                      ""HumidityValue"": " + (42.475M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""HumidityValueTime"": """ + x.Item2 + @"""
                                                  }")) + @"
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
                                              ""ZoneID"": 0
                                          },
                                          {
                                              ""ZoneID"": 1
                                          },
                                          {
                                              ""ZoneID"": 32027,
                                              ""sensor"": [" + string.Join(',', Enumerable.Range(0, valueCount)
                                                  .Select(x => new Tuple<int, long>(x, new DateTimeOffset(DateTime.UtcNow.AddSeconds(-1 * resolution * (valueCount - x))).ToUnixTimeSeconds()))
                                                  .Select(x => @"
                                                  {
                                                      ""type"": 9,
                                                      ""value"": " + (21.3M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""time"": " + x.Item2 + @"
                                                  },
                                                  {
                                                      ""type"": 13,
                                                      ""value"": " + (45.975M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""time"": " + x.Item2 + @"
                                                  }")) + @"
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            return mockHttp;
        }

        public static MockHttpMessageHandler AddEnergyMeteringMocks(this MockHttpMessageHandler mockHttp, int resolution = 1, int valueCount = 600)
        {
            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/dSMeters/*(dSUID,name)/capabilities(metering)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""dSMeters"": [
                                          {
                                              ""dSUID"": ""99999942f800000000000f0000deadbeef"",
                                              ""name"": ""dss meter"",
                                              ""capabilities"": [
                                                  {
                                                      ""metering"": true
                                                  }
                                              ]
                                          },
                                          {
                                              ""dSUID"": ""77777742f800000000000c0000cafecafe"",
                                              ""name"": ""dss meter"",
                                              ""capabilities"": [
                                                  {
                                                      ""metering"": true
                                                  }
                                              ]
                                          },
                                          {
                                              ""dSUID"": ""111111111800000000000f00000000abcd"",
                                              ""name"": ""vdc meter"",
                                              ""capabilities"": [
                                                  {
                                                      ""metering"": false
                                                  }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            foreach (var dsuid in new List<string>() { "99999942f800000000000f0000deadbeef", "77777742f800000000000c0000cafecafe" })
            {
                mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/metering/getValues")
                        .WithExactQueryString($"dsuid={dsuid}&type=consumption&resolution={resolution}&valueCount={valueCount}&token={MockDigitalstromConnection.AppToken}")
                        .Respond("application/json", @"{
                                      ""result"":
                                      {
                                          ""meterID"": """ + dsuid + @""",
                                          ""type"": ""consumption"",
                                          ""unit"": ""W"",
                                          ""resolution"": ""1"",
                                          ""values"": [" + string.Join(',', Enumerable.Range(0, valueCount).Select(x => @"
                                              [
                                                  " + new DateTimeOffset(DateTime.UtcNow.AddSeconds(-1 * resolution * (valueCount - x))).ToUnixTimeSeconds() + @",
                                                  " + (dsuid.StartsWith("9") ? 89 + x : 7 + x * 2) + @"
                                              ]")) + @"
                                          ]
                                      },
                                      ""ok"": true
                                  }");
            }

            return mockHttp;
        }

        public static MockHttpMessageHandler AddStructureMock(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/apartment/getStructure")
                    .WithExactQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""apartment"": {
                                          ""clusters"": [
                                              {
                                                  ""id"": 16,
                                                  ""name"": ""South - none"",
                                                  ""color"": 2,
                                                  ""applicationType"": 2,
                                                  ""isPresent"": true,
                                                  ""isValid"": true,
                                                  ""CardinalDirection"": ""south"",
                                                  ""ProtectionClass"": 0,
                                                  ""isAutomatic"": true,
                                                  ""configurationLock"": false,
                                                  ""devices"": [
                                                      ""13372342f800000000000f0000deadbeef""
                                                  ],
                                                  ""lockEvents"": []
                                              }
                                          ],
                                          ""zones"": [
                                              {
                                                  ""id"": 32027,
                                                  ""name"": ""Kitchen"",
                                                  ""isPresent"": true,
                                                  ""devices"": [
                                                      {
                                                          ""id"": ""1337234200000e80deadbeef"",
                                                          ""DisplayID"": ""00037e9b"",
                                                          ""dSUID"": ""13372342f800000000000f0000deadbeef"",
                                                          ""GTIN"": ""7123456789012"",
                                                          ""name"": ""MyDevice"",
                                                          ""dSUIDIndex"": 0,
                                                          ""functionID"": 12565,
                                                          ""productRevision"": 865,
                                                          ""productID"": 200,
                                                          ""hwInfo"": ""BL-KM200"",
                                                          ""OemStatus"": ""None"",
                                                          ""OemEanNumber"": ""0"",
                                                          ""OemSerialNumber"": 0,
                                                          ""OemPartNumber"": 0,
                                                          ""OemProductInfoState"": ""Unknown"",
                                                          ""OemProductURL"": """",
                                                          ""OemInternetState"": ""No EAN"",
                                                          ""OemIsIndependent"": true,
                                                          ""modelFeatures"": {},
                                                          ""isVdcDevice"": false,
                                                          ""supportedBasicScenes"": [],
                                                          ""ValveType"": ""Unknown"",
                                                          ""meterDSID"": ""302ed89f43f00e4000007d19"",
                                                          ""meterDSUID"": ""99999942f800000000000f0000deadbeef"",
                                                          ""meterName"": ""MyMeter"",
                                                          ""busID"": 397,
                                                          ""zoneID"": 32027,
                                                          ""isPresent"": true,
                                                          ""isValid"": true,
                                                          ""lastDiscovered"": ""2019-04-19 22:33:29"",
                                                          ""firstSeen"": ""2019-02-05 12:00:00"",
                                                          ""inactiveSince"": ""1970-01-01 01:00:00"",
                                                          ""on"": true,
                                                          ""locked"": false,
                                                          ""configurationLocked"": false,
                                                          ""ignoreOperationLock"": false,
                                                          ""outputMode"": 64,
                                                          ""buttonID"": 0,
                                                          ""buttonActiveGroup"": 3,
                                                          ""buttonGroupMembership"": 3,
                                                          ""buttonInputMode"": 65,
                                                          ""buttonInputIndex"": 0,
                                                          ""buttonInputCount"": 1,
                                                          ""AKMInputProperty"": """",
                                                          ""groups"": [
                                                              48
                                                          ],
                                                          ""binaryInputCount"": 0,
                                                          ""binaryInputs"": [],
                                                          ""sensorInputCount"": 5,
                                                          ""sensors"": [
                                                              {
                                                                  ""type"": 253,
                                                                  ""valid"": false,
                                                                  ""value"": 0
                                                              }
                                                          ],
                                                          ""sensorDataValid"": true,
                                                          ""outputChannels"": [],
                                                          ""pairedDevices"": []
                                                      }
                                                  ],
                                                  ""groups"": [
                                                      {
                                                          ""id"": 10,
                                                          ""name"": ""ventilation"",
                                                          ""color"": 3,
                                                          ""applicationType"": 10,
                                                          ""isPresent"": true,
                                                          ""isValid"": true,
                                                          ""activeBasicScenes"": [ 0, 5, 17, 18, 19, 6 ],
                                                          ""devices"": []
                                                      },
                                                      {
                                                          ""id"": 48,
                                                          ""name"": ""controltemperature"",
                                                          ""color"": 3,
                                                          ""applicationType"": 48,
                                                          ""isPresent"": true,
                                                          ""isValid"": true,
                                                          ""devices"": [
                                                              ""13372342f800000000000f0000deadbeef""
                                                          ]
                                                      }
                                                  ]
                                              }
                                          ]
                                      }
                                  },
                                  ""ok"": true
                              }");

            return mockHttp;
        }

        public static MockHttpMessageHandler AddCircuitZonesMocks(this MockHttpMessageHandler mockHttp, IEnumerable<Zone> zoneIds)
        {
            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/dSMeters/*(dSUID)/zones/*(ZoneID)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""dSMeters"": [
                                          {
                                              ""dSUID"": ""99999942f800000000000f0000deadbeef"",
                                              ""zones"": [" + string.Join(',', zoneIds.Select(z =>
                                                  @"{
                                                      ""ZoneID"": " + ((int)z).ToString() + @"
                                                  }"
                                              )) + @"]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            return mockHttp;
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

        public static MockedRequest AddCallSceneMock(this MockHttpMessageHandler mockHttp, Zone zone, Group group, Scene scene, bool force = false)
        {
            var forceStr = force ? "&force=true" : string.Empty;
            return mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/zone/callScene")
                    .WithExactQueryString($"id={(int)zone}&groupID={(int)group}&sceneNumber={(int)scene}{forceStr}&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");
        }

        public static string ToMockedSceneEvent(this SceneCommand scene)
        {
            return ((Scene)scene).ToMockedSceneEvent(32027, 1);
        }

        public static string ToMockedSceneEvent(this Scene scene, Zone zone, Group group)
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
                                         ""groupID"": """ + ((int)group).ToString() + @""",
                                         ""zoneID"": """ + ((int)zone).ToString() + @""",
                                         ""originToken"": ""5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2"",
                                         ""originDSUID"": ""0000000000000000000000000000000000""
                                     },
                                     ""source"": {
                                         ""set"": "".zone(" + ((int)zone).ToString() + @").group(" + ((int)group).ToString() + @")"",
                                         ""groupID"": " + ((int)group).ToString() + @",
                                         ""zoneID"": " + ((int)zone).ToString() + @",
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