using NodaTime;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromDssMock;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Tests
{
    public class DigitalstromDssClientTest
    {
        private readonly Zone zoneKitchen = 32027;

        [Fact]
        public async Task TestGetSensorValues()
        {
            var dsApiClient = new DigitalstromDssClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddSensorMocks(1, 1)
                .ToMockProvider());

            var result = await dsApiClient.GetSensorValues();

            Assert.Equal("01n", result.Weather.WeatherIconId);
            Assert.Equal("800", result.Weather.WeatherConditionId);
            Assert.Equal("7", result.Weather.WeatherServiceId);
            Assert.Equal("2019-05-01T21:15:07.560Z", result.Weather.WeatherServiceTime);

            Assert.Null(result.Outdoor.Temperature);
            Assert.Null(result.Outdoor.Humidity);
            Assert.Null(result.Outdoor.Brightness);
            Assert.Null(result.Outdoor.Precipitation);
            Assert.Null(result.Outdoor.Airpressure);
            Assert.Null(result.Outdoor.Windspeed);
            Assert.Null(result.Outdoor.Winddirection);
            Assert.Null(result.Outdoor.Gustspeed);
            Assert.Null(result.Outdoor.Gustdirection);

            Assert.Equal(20.05, result.Zones[1].Values[0].TemperatureValue);
            Assert.Equal(42.475, result.Zones[1].Values[1].HumidityValue);

            Assert.Equal(20.05, result.Zones[1].Temperature.Value);
            Assert.Equal(42.475, result.Zones[1].Humidity.Value);
            Assert.Null(result.Zones[1].Brightness);
            Assert.Null(result.Zones[1].Co2concentration);
        }

        [Fact]
        public async Task TestGetStructure()
        {
            var mockHttp = new MockHttpMessageHandler();

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

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetStructure();

            Assert.Equal("South - none", result.Clusters[0].Name);
            Assert.Equal(2, result.Clusters[0].ApplicationType);
            Assert.True(result.Clusters[0].IsPresent);

            Assert.Equal(32027, (int)result.Zones[0].Id);
            Assert.True(result.Zones[0].IsPresent);

            Assert.Equal("1337234200000e80deadbeef", result.Zones[0].Devices[0].Id);
            Assert.Equal("MyDevice", result.Zones[0].Devices[0].Name);
            Assert.Equal("13372342f800000000000f0000deadbeef", result.Zones[0].Devices[0].DSUID);
            Assert.Equal("99999942f800000000000f0000deadbeef", result.Zones[0].Devices[0].MeterDSUID);
            Assert.Equal(new DateTime(2019, 04, 19, 22, 33, 29), result.Zones[0].Devices[0].LastDiscovered);
            Assert.Equal(new List<int>() { 48 }.Select(x => (Group)x), result.Zones[0].Devices[0].Groups);

            Assert.Equal(253, (int)result.Zones[0].Devices[0].Sensors[0].Type);
            Assert.Equal(0, result.Zones[0].Devices[0].Sensors[0].Value);
            Assert.False(result.Zones[0].Devices[0].Sensors[0].Valid);

            Assert.Equal(3, (int)result.Zones[0].Groups[0].Color);
            Assert.True(result.Zones[0].Groups[0].IsValid);
            Assert.Equal(new List<int>() { 0, 5, 17, 18, 19, 6 }.Select(x => (Scene)x), result.Zones[0].Groups[0].ActiveBasicScenes);
        }

        [Fact]
        public async Task TestGetTemperatureControlStatus()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/apartment/getTemperatureControlStatus")
                    .WithExactQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"": [
                                          {
                                              ""id"": 1,
                                              ""name"": ""RoomWithoutControl"",
                                              ""ControlMode"": 0,
                                              ""ControlState"": 0
                                          },
                                          {
                                              ""id"": 32001,
                                              ""name"": ""RoomFollowing"",
                                              ""ControlMode"": 2,
                                              ""ControlState"": 0,
                                              ""ControlValue"": 18,
                                              ""ControlValueTime"": ""2019-05-02T09:12:27Z""
                                          },
                                          {
                                              ""id"": 32027,
                                              ""name"": ""RoomWithControl"",
                                              ""ControlMode"": 1,
                                              ""ControlState"": 0,
                                              ""OperationMode"": 1,
                                              ""TemperatureValue"": 19.925,
                                              ""TemperatureValueTime"": ""2019-05-02T09:12:43Z"",
                                              ""NominalValue"": 20,
                                              ""NominalValueTime"": ""2018-12-08T18:10:44Z"",
                                              ""ControlValue"": 93,
                                              ""ControlValueTime"": ""2019-05-02T09:12:53Z""
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetTemperatureControlStatus();

            Assert.Equal(32027, (int)result.Zones[2].Id);
            Assert.Equal("RoomWithControl", result.Zones[2].Name);
            Assert.Equal(1, result.Zones[2].ControlMode);
            Assert.Equal(19.925, result.Zones[2].TemperatureValue);
            Assert.Equal(new DateTime(2019, 5, 2, 9, 12, 43, DateTimeKind.Utc), result.Zones[2].TemperatureValueTime);
            Assert.Equal(20, result.Zones[2].NominalValue);
            Assert.Equal(new DateTime(2018, 12, 8, 18, 10, 44, DateTimeKind.Utc), result.Zones[2].NominalValueTime);
            Assert.Equal(93, result.Zones[2].ControlValue);
            Assert.Equal(new DateTime(2019, 5, 2, 9, 12, 53, DateTimeKind.Utc), result.Zones[2].ControlValueTime);
        }

        [Fact]
        public async Task TestGetTemperatureControlValues()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/apartment/getTemperatureControlValues")
                    .WithExactQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"": [
                                          {
                                              ""id"": 1,
                                              ""name"": ""RoomWithoutControl""
                                          },
                                          {
                                              ""id"": 32027,
                                              ""name"": ""RoomWithControl"",
                                              ""Off"": 8,
                                              ""Comfort"": 21,
                                              ""Economy"": 20,
                                              ""NotUsed"": 18,
                                              ""Night"": 17,
                                              ""Holiday"": 16,
                                              ""Cooling"": 23,
                                              ""CoolingOff"": 35
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetTemperatureControlValues();

            Assert.Equal(32027, (int)result.Zones[1].Id);
            Assert.Equal("RoomWithControl", result.Zones[1].Name);
            Assert.Equal(8, result.Zones[1].Off);
            Assert.Equal(21, result.Zones[1].Comfort);
            Assert.Equal(20, result.Zones[1].Economy);

            Assert.Null(result.Zones[0].Off);
            Assert.Null(result.Zones[0].Comfort);
            Assert.Null(result.Zones[0].Economy);
        }

        [Fact]
        public async Task TestGetTemperatureControlConfig()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/apartment/getTemperatureControlConfig")
                    .WithExactQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"": [
                                          {
                                              ""id"": 1,
                                              ""name"": ""RoomWithoutControl"",
                                              ""ControlMode"": 0
                                          },
                                          {
                                              ""id"": 32001,
                                              ""name"": ""RoomFollowing"",
                                              ""ControlMode"": 2,
                                              ""ReferenceZone"": 32027,
                                              ""CtrlOffset"": 0
                                          },
                                          {
                                              ""id"": 32027,
                                              ""name"": ""RoomWithControl"",
                                              ""ControlMode"": 1,
                                              ""EmergencyValue"": 75,
                                              ""CtrlKp"": 5,
                                              ""CtrlTs"": 1,
                                              ""CtrlTi"": 240,
                                              ""CtrlKd"": 0,
                                              ""CtrlImin"": -13.325000000000001,
                                              ""CtrlImax"": 13.325000000000001,
                                              ""CtrlYmin"": 0,
                                              ""CtrlYmax"": 100,
                                              ""CtrlAntiWindUp"": true
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetTemperatureControlConfig();

            Assert.Equal(0, result.Zones[0].ControlMode);
            Assert.Null(result.Zones[0].ReferenceZone);
            Assert.Null(result.Zones[0].CtrlOffset);

            Assert.Equal(2, result.Zones[1].ControlMode);
            Assert.Equal(32027, result.Zones[1].ReferenceZone);
            Assert.Equal(0, result.Zones[1].CtrlOffset);
            Assert.Null(result.Zones[1].CtrlKp);

            Assert.Equal(32027, (int)result.Zones[2].Id);
            Assert.Equal(1, result.Zones[2].ControlMode);
            Assert.Equal(75, result.Zones[2].EmergencyValue);
            Assert.Equal(5, result.Zones[2].CtrlKp);
            Assert.Equal(1, result.Zones[2].CtrlTs);
            Assert.Equal(240, result.Zones[2].CtrlTi);
            Assert.Null(result.Zones[2].ReferenceZone);
        }

        [Fact]
        public async Task TestSetTemperatureControlValues()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/zone/setTemperatureControlValues")
                    .WithExactQueryString($"id=32027&Night=22&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""ok"": false,
                                  ""message"": ""Cannot set control values in current mode""
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            await Assert.ThrowsAsync<IOException>(() => dsApiClient.SetTemperatureControlValues(zoneKitchen, null, null, 22));
        }

        [Fact]
        public async Task TestCallScene()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/zone/callScene")
                    .WithExactQueryString($"id=32027&groupID=1&sceneNumber=5&force=true&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.CallScene(zoneKitchen, Color.Yellow, SceneCommand.Preset1, true);
        }

        [Fact]
        public async Task TestGetReachableScenes()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/zone/getReachableScenes")
                    .WithExactQueryString($"id=32027&groupID=1&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""reachableScenes"": [ 0, 5, 17, 18, 19 ],
                                      ""userSceneNames"": []
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetReachableScenes(zoneKitchen, Color.Yellow);

            Assert.Equal(new List<int>() { 0, 5, 17, 18, 19 }.Select(x => (Scene)x), result.ReachableScenes);
            Assert.Equal(new List<string>(), result.UserSceneNames);
        }

        [Fact]
        public async Task TestGetLastCalledScene()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/zone/getLastCalledScene")
                    .WithExactQueryString($"id=32027&groupID=1&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""scene"": 5
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetLastCalledScene(zoneKitchen, Color.Yellow);

            Assert.Equal(5, (int)result.Scene);
        }

        [Fact]
        public async Task TestGetZonesAndLastCalledScenes()
        {
            var mockHttp = new MockHttpMessageHandler();

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

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var zonesScenes = await dsApiClient.GetZonesAndLastCalledScenes();
            Assert.NotEmpty(zonesScenes.Zones);

            Assert.Equal(5, (int)zonesScenes.Zones[0].Groups.First(g => g.Group == 4).LastCalledScene);
            Assert.Equal(40, (int)zonesScenes.Zones[0].Groups.First(g => g.Group == 8).LastCalledScene);
        }

        [Fact]
        public async Task TestGetDevicesAndOutputChannelTypes()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/zones/*(ZoneID)/devices/*(dSID)/outputChannels/*(id)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"":
                                      [
                                          {
                                              ""ZoneID"": 32027,
                                              ""devices"":
                                              [
                                                  {
                                                      ""dSID"": ""1337234200000e80deadbeef"",
                                                      ""outputChannels"":
                                                      [
                                                          {
                                                              ""id"": ""brightness""
                                                          }
                                                      ]
                                                  },
                                                  {
                                                      ""dSID"": ""4242000aaa000fa0deadbeef"",
                                                      ""outputChannels"":
                                                      [
                                                          {
                                                              ""id"": ""shadePositionOutside""
                                                          }
                                                      ]
                                                  },
                                                  {
                                                      ""dSID"": ""f00f000aaa000120beefbeef"",
                                                      ""outputChannels"": []
                                                  }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var zonesScenes = await dsApiClient.GetDevicesAndOutputChannelTypes();
            Assert.NotEmpty(zonesScenes.Zones);

            Assert.Equal("brightness", zonesScenes.Zones[0].Devices[0].OutputChannels[0].Id);
            Assert.Equal("shadePositionOutside", zonesScenes.Zones[0].Devices[1].OutputChannels[0].Id);
            Assert.Empty(zonesScenes.Zones[0].Devices[2].OutputChannels);
        }

        [Fact]
        public async Task TestGetDevicesAndLastOutputValues()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/zones/*(ZoneID)/devices/*(dSID)/status(lastChanged)/outputs/*(value,targetValue)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"":
                                      [
                                          {
                                              ""ZoneID"": 32027,
                                              ""devices"":
                                              [
                                                  {
                                                      ""dSID"": ""1337234200000e80deadbeef"",
                                                      ""status"":
                                                      [
                                                          {
                                                              ""lastChanged"": ""\""2019-01-15T03:56:29.800Z\"""",
                                                              ""outputs"":
                                                              [
                                                                  {
                                                                      ""value"": 0,
                                                                      ""targetValue"": 0
                                                                  }
                                                              ]
                                                          }
                                                      ]
                                                  },
                                                  {
                                                      ""dSID"": ""4242000aaa000fa0deadbeef"",
                                                      ""status"":
                                                      [
                                                          {
                                                              ""lastChanged"": ""\""2019-01-18T17:15:30.249Z\"""",
                                                              ""outputs"":
                                                              [
                                                                  {
                                                                      ""value"": 21.6404131546947,
                                                                      ""targetValue"": 21.6404131546947
                                                                  }
                                                              ]
                                                          }
                                                      ]
                                                  },
                                                  {
                                                      ""dSID"": ""f00f000aaa000120beefbeef"",
                                                      ""status"": []
                                                  }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var zonesScenes = await dsApiClient.GetDevicesAndLastOutputValues();
            Assert.NotEmpty(zonesScenes.Zones);

            Assert.Equal(DateTime.Parse("2019-01-15T03:56:29.800Z"), zonesScenes.Zones[0].Devices[0].Status[0].LastChangedTime);
            Assert.Equal(0d, zonesScenes.Zones[0].Devices[0].Status[0].Outputs[0].Value);
            Assert.Equal(0d, zonesScenes.Zones[0].Devices[0].Status[0].Outputs[0].TargetValue);
            Assert.Equal(21.6404131546947d, zonesScenes.Zones[0].Devices[1].Status[0].Outputs[0].Value);
            Assert.Equal(21.6404131546947d, zonesScenes.Zones[0].Devices[1].Status[0].Outputs[0].TargetValue);
            Assert.Empty(zonesScenes.Zones[0].Devices[2].Status);
        }

        [Fact]
        public async Task TestGetZonesAndSensorValues()
        {
            var dsApiClient = new DigitalstromDssClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddSensorMocks()
                .ToMockProvider());

            var result = await dsApiClient.GetZonesAndSensorValues();

            Assert.Equal(0, (int)result.Zones[0].ZoneID);
            Assert.Null(result.Zones[0].Sensor);

            Assert.Equal(32027, (int)result.Zones[2].ZoneID);
            Assert.Equal(9, (int)result.Zones[2].Sensor[0].Type);
            Assert.Equal(21.3, result.Zones[2].Sensor[0].Value);
        }

        [Fact]
        public async Task TestGetMeteringCircuits()
        {
            var dsApiClient = new DigitalstromDssClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddEnergyMeteringMocks()
                .ToMockProvider());

            var result = await dsApiClient.GetMeteringCircuits();

            Assert.Equal("99999942f800000000000f0000deadbeef", result.DSMeters[0].DSUID);
            Assert.True(result.DSMeters[0].Capabilities[0].Metering);
            Assert.Equal("77777742f800000000000c0000cafecafe", result.DSMeters[1].DSUID);
            Assert.True(result.DSMeters[1].Capabilities[0].Metering);
            Assert.False(result.DSMeters[2].Capabilities[0].Metering);

            Assert.Equal(2, result.FilteredMeterNames.Count);
        }

        [Fact]
        public async Task TestGetCircuitZones()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/dSMeters/*(dSUID)/zones/*(ZoneID)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""dSMeters"": [
                                          {
                                              ""dSUID"": ""99999942f800000000000f0000deadbeef"",
                                              ""zones"": [
                                                  {
                                                      ""ZoneID"": 4
                                                  },
                                                  {
                                                      ""ZoneID"": 32027
                                                  },
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetCircuitZones();

            Assert.Equal("99999942f800000000000f0000deadbeef", result.DSMeters[0].DSUID);
            Assert.Equal(2, result.DSMeters[0].Zones.Count);
            Assert.Equal(4, (int)result.DSMeters[0].Zones[0].ZoneID);
            Assert.Equal(32027, (int)result.DSMeters[0].Zones[1].ZoneID);
        }

        [Fact]
        public async Task TestGetTotalEnergy()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/metering/getValues")
                    .WithExactQueryString($"dsuid=.meters(all)&type=consumption&resolution=1&valueCount=2&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""meterID"": [
                                          ""99999942f800000000000f0000deadbeef"",
                                          ""789456789000000000000f0000feeefacb""
                                      ],
                                      ""type"": ""consumption"",
                                      ""unit"": ""W"",
                                      ""resolution"": ""1"",
                                      ""values"": [
                                          [
                                              1556788953,
                                              206
                                          ],
                                          [
                                              1556788954,
                                              212
                                          ]
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetTotalEnergy(1, 2);

            Assert.Equal("consumption", result.Type);
            Assert.Equal("W", result.Unit);
            Assert.Equal(1, result.Resolution);
            Assert.Equal(2, result.Values.Count);
            Assert.Equal(2, result.Values[0].Count);
            Assert.Equal(1556788953, result.Values[0][0]);
            Assert.Equal(206, result.Values[0][1]);
        }

        [Fact]
        public async Task TestGetEnergy()
        {
            var dsApiClient = new DigitalstromDssClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddEnergyMeteringMocks(1, 50)
                .ToMockProvider());

            var result = await dsApiClient.GetEnergy(new Dsuid("99999942f800000000000f0000deadbeef"), 1, 50);

            Assert.Equal("consumption", result.Type);
            Assert.Equal("W", result.Unit);
            Assert.Equal(1, result.Resolution);
            Assert.Equal(50, result.Values.Count);
            Assert.Equal(2, result.Values[0].Count);
            Assert.Equal(Instant.FromDateTimeUtc(DateTime.UtcNow.AddSeconds(-50)).ToUnixTimeSeconds(), result.Values[0][0]);
            Assert.Equal(89, result.Values[0][1]);
        }

        [Fact]
        public async Task TestSubscribe()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/subscribe")
                    .WithExactQueryString($"name=callScene&subscriptionID=42&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.Subscribe((SystemEventName)SystemEvent.CallScene, 42);
        }

        [Fact]
        public async Task TestUnsubscribe()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/unsubscribe")
                    .WithExactQueryString($"name=callScene&subscriptionID=42&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.Unsubscribe((SystemEventName)SystemEvent.CallScene, 42);
        }

        [Fact]
        public async Task TestPollForEvents()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                    .WithExactQueryString($"subscriptionID=42&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""events"": [
                                          {
                                              ""name"": ""callScene"",
                                              ""properties"": {
                                                  ""callOrigin"": ""2"",
                                                  ""sceneID"": ""5"",
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
                              }");

            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.PollForEvents(42, 60000);

            Assert.Equal("callScene", result.Events[0].Name);
            Assert.Equal("2", result.Events[0].Properties.CallOrigin);
            Assert.Equal("5", result.Events[0].Properties.SceneID);
            Assert.True(result.Events[0].Properties.Forced);
            Assert.Equal("32027", result.Events[0].Properties.ZoneID);
            Assert.Equal("5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2", result.Events[0].Properties.OriginToken);
            Assert.Equal("0000000000000000000000000000000000", result.Events[0].Properties.OriginDSUID);
            Assert.Equal(".zone(32027).group(1)", result.Events[0].Source.Set);
            Assert.Equal(1, (int)result.Events[0].Source.GroupID);
            Assert.Equal(32027, (int)result.Events[0].Source.ZoneID);
            Assert.False(result.Events[0].Source.IsApartment);
            Assert.True(result.Events[0].Source.IsGroup);
            Assert.False(result.Events[0].Source.IsDevice);
        }

        [Fact]
        public async Task TestRaiseEvent()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/raise")
                    .WithExactQueryString($"name=callScene&parameter=mykey=myval&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""ok"": true
                              }");
            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.RaiseEvent((SystemEventName)SystemEvent.CallScene,
                new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("mykey", "myval") });
        }

        [Fact]
        public async Task GenerateUnitTestRequestUris()
        {
            var mockHttp = new MockDigitalstromConnection.TestGenerationHttpMessageHandler();
            var dsApiClient = new DigitalstromDssClient(mockHttp.AddAuthMock().ToTestGenerationProvider());

            var UriForMethodName = new Dictionary<string, string>();

            try { await dsApiClient.GetSensorValues(); } catch { }
            UriForMethodName.Add("GetSensorValues", mockHttp.LastCalledUri);

            try { await dsApiClient.GetStructure(); } catch { }
            UriForMethodName.Add("GetStructure", mockHttp.LastCalledUri);

            try { await dsApiClient.GetTemperatureControlStatus(); } catch { }
            UriForMethodName.Add("GetTemperatureControlStatus", mockHttp.LastCalledUri);

            try { await dsApiClient.GetTemperatureControlValues(); } catch { }
            UriForMethodName.Add("GetTemperatureControlValues", mockHttp.LastCalledUri);

            try { await dsApiClient.GetTemperatureControlConfig(); } catch { }
            UriForMethodName.Add("GetTemperatureControlConfig", mockHttp.LastCalledUri);

            try { await dsApiClient.SetTemperatureControlValues(zoneKitchen, null, null, 22); } catch { }
            UriForMethodName.Add("SetTemperatureControlValues", mockHttp.LastCalledUri);

            try { await dsApiClient.CallScene(zoneKitchen, Color.Yellow, SceneCommand.Preset1, true); } catch { }
            UriForMethodName.Add("CallScene", mockHttp.LastCalledUri);

            try { await dsApiClient.GetReachableScenes(zoneKitchen, Color.Yellow); } catch { }
            UriForMethodName.Add("GetReachableScenes", mockHttp.LastCalledUri);

            try { await dsApiClient.GetLastCalledScene(zoneKitchen, Color.Yellow); } catch { }
            UriForMethodName.Add("GetLastCalledScene", mockHttp.LastCalledUri);

            try { await dsApiClient.GetZonesAndLastCalledScenes(); } catch { }
            UriForMethodName.Add("GetZonesAndLastCalledScenes", mockHttp.LastCalledUri);

            try { await dsApiClient.GetDevicesAndOutputChannelTypes(); } catch { }
            UriForMethodName.Add("GetDevicesAndOutputChannelTypes", mockHttp.LastCalledUri);

            try { await dsApiClient.GetDevicesAndLastOutputValues(); } catch { }
            UriForMethodName.Add("GetDevicesAndLastOutputValues", mockHttp.LastCalledUri);

            try { await dsApiClient.GetZonesAndSensorValues(); } catch { }
            UriForMethodName.Add("GetZonesAndSensorValues", mockHttp.LastCalledUri);

            try { await dsApiClient.GetMeteringCircuits(); } catch { }
            UriForMethodName.Add("GetMeteringCircuits", mockHttp.LastCalledUri);

            try { await dsApiClient.GetCircuitZones(); } catch { }
            UriForMethodName.Add("GetCircuitZones", mockHttp.LastCalledUri);

            try { await dsApiClient.GetTotalEnergy(1, 600); } catch { }
            UriForMethodName.Add("GetTotalEnergy", mockHttp.LastCalledUri);

            try { await dsApiClient.GetEnergy(new Dsuid("99999942f800000000000f0000deadbeef"), 1, 600); } catch { }
            UriForMethodName.Add("GetEnergy", mockHttp.LastCalledUri);

            try { await dsApiClient.Subscribe((SystemEventName)SystemEvent.CallScene, 42); } catch { }
            UriForMethodName.Add("Subscribe", mockHttp.LastCalledUri);

            try { await dsApiClient.Unsubscribe((SystemEventName)SystemEvent.CallScene, 42); } catch { }
            UriForMethodName.Add("Unsubscribe", mockHttp.LastCalledUri);

            try { await dsApiClient.PollForEvents(42, 60000); } catch { }
            UriForMethodName.Add("PollForEvents", mockHttp.LastCalledUri);

            try { await dsApiClient.RaiseEvent((SystemEventName)SystemEvent.CallScene,
                new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("mykey", "myval") }); } catch { }
            UriForMethodName.Add("RaiseEvent", mockHttp.LastCalledUri);

            var allUris = string.Join("\n", UriForMethodName.Select(x => $"{x.Key}: {x.Value}"));
        }
    }
}