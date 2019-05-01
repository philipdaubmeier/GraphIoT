using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Network;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Tests
{
    public class DigitalstromWebserviceClientTest
    {
        private Zone zoneKitchen = 32027;

        [Fact]
        public async Task TestGetSensorValues()
        {
            var mockHttp = new MockHttpMessageHandler();

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
                                              ""id"": 32027,
                                              ""name"": ""Kitchen"",
                                              ""values"": [
                                                  {
                                                      ""TemperatureValue"": 20.05,
                                                      ""TemperatureValueTime"": ""2019-05-01T21:17:01.319Z""
                                                  },
                                                  {
                                                      ""HumidityValue"": 42.475,
                                                      ""HumidityValueTime"": ""2019-05-01T21:02:12.988Z""
                                                  }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetSensorValues();

            Assert.Equal("01n", result.weather.WeatherIconId);
            Assert.Equal("800", result.weather.WeatherConditionId);
            Assert.Equal("7", result.weather.WeatherServiceId);
            Assert.Equal("2019-05-01T21:15:07.560Z", result.weather.WeatherServiceTime);

            Assert.Null(result.outdoor.temperature);
            Assert.Null(result.outdoor.humidity);
            Assert.Null(result.outdoor.brightness);
            Assert.Null(result.outdoor.precipitation);
            Assert.Null(result.outdoor.airpressure);
            Assert.Null(result.outdoor.windspeed);
            Assert.Null(result.outdoor.winddirection);
            Assert.Null(result.outdoor.gustspeed);
            Assert.Null(result.outdoor.gustdirection);

            Assert.Equal(20.05, result.zones[1].values[0].TemperatureValue);
            Assert.Equal(42.475, result.zones[1].values[1].HumidityValue);

            Assert.Equal(20.05, result.zones[1].temperature.value);
            Assert.Equal(42.475, result.zones[1].humidity.value);
            Assert.Null(result.zones[1].brightness);
            Assert.Null(result.zones[1].co2concentration);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetStructure();

            Assert.Equal("South - none", result.apartment.clusters[0].name);
            Assert.Equal(2, result.apartment.clusters[0].applicationType);
            Assert.True(result.apartment.clusters[0].isPresent);

            Assert.Equal(32027, result.apartment.zones[0].id);
            Assert.True(result.apartment.zones[0].isPresent);

            Assert.Equal("1337234200000e80deadbeef", result.apartment.zones[0].devices[0].id);
            Assert.Equal("MyDevice", result.apartment.zones[0].devices[0].name);
            Assert.Equal("13372342f800000000000f0000deadbeef", result.apartment.zones[0].devices[0].dSUID);
            Assert.Equal("99999942f800000000000f0000deadbeef", result.apartment.zones[0].devices[0].meterDSUID);
            Assert.Equal(new List<int>() { 48 }, result.apartment.zones[0].devices[0].groups);

            Assert.Equal(253, result.apartment.zones[0].devices[0].sensors[0].type);
            Assert.Equal(0, result.apartment.zones[0].devices[0].sensors[0].value);
            Assert.False(result.apartment.zones[0].devices[0].sensors[0].valid);

            Assert.Equal(3, result.apartment.zones[0].groups[0].color);
            Assert.True(result.apartment.zones[0].groups[0].isValid);
            Assert.Equal(new List<int?>() { 0, 5, 17, 18, 19, 6 }, result.apartment.zones[0].groups[0].activeBasicScenes);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetTemperatureControlStatus();

            Assert.Equal(32027, result.zones[2].id);
            Assert.Equal("RoomWithControl", result.zones[2].name);
            Assert.Equal(1, result.zones[2].ControlMode);
            Assert.Equal(19.925, result.zones[2].TemperatureValue);
            Assert.Equal("2019-05-02T09:12:43Z", result.zones[2].TemperatureValueTime);
            Assert.Equal(20, result.zones[2].NominalValue);
            Assert.Equal("2018-12-08T18:10:44Z", result.zones[2].NominalValueTime);
            Assert.Equal(93, result.zones[2].ControlValue);
            Assert.Equal("2019-05-02T09:12:53Z", result.zones[2].ControlValueTime);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetTemperatureControlValues();

            Assert.Equal(32027, result.zones[1].id);
            Assert.Equal("RoomWithControl", result.zones[1].name);
            Assert.Equal(8, result.zones[1].Off);
            Assert.Equal(21, result.zones[1].Comfort);
            Assert.Equal(20, result.zones[1].Economy);

            Assert.Null(result.zones[0].Off);
            Assert.Null(result.zones[0].Comfort);
            Assert.Null(result.zones[0].Economy);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetTemperatureControlConfig();

            Assert.Equal(0, result.zones[0].ControlMode);
            Assert.Null(result.zones[0].ReferenceZone);
            Assert.Null(result.zones[0].CtrlOffset);

            Assert.Equal(2, result.zones[1].ControlMode);
            Assert.Equal(32027, result.zones[1].ReferenceZone);
            Assert.Equal(0, result.zones[1].CtrlOffset);
            Assert.Null(result.zones[1].CtrlKp);

            Assert.Equal(32027, result.zones[2].id);
            Assert.Equal(1, result.zones[2].ControlMode);
            Assert.Equal(75, result.zones[2].EmergencyValue);
            Assert.Equal(5, result.zones[2].CtrlKp);
            Assert.Equal(1, result.zones[2].CtrlTs);
            Assert.Equal(240, result.zones[2].CtrlTi);
            Assert.Null(result.zones[2].ReferenceZone);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.CallScene(zoneKitchen, (Group)Group.Color.Yellow, (Scene)Scene.SceneCommand.Preset1, true);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetReachableScenes(zoneKitchen, (Group)Group.Color.Yellow);

            Assert.Equal(new List<int>() { 0, 5, 17, 18, 19 }, result.reachableScenes);
            Assert.Equal(new List<string>(), result.userSceneNames);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetLastCalledScene(zoneKitchen, (Group)Group.Color.Yellow);

            Assert.Equal(5, result.scene);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var zonesScenes = await dsApiClient.GetZonesAndLastCalledScenes();
            Assert.NotEmpty(zonesScenes.zones);

            Assert.Equal(5, zonesScenes.zones[0].groups.First(g => g.group == 4).lastCalledScene);
            Assert.Equal(40, zonesScenes.zones[0].groups.First(g => g.group == 8).lastCalledScene);
        }

        [Fact]
        public async Task TestGetZonesAndSensorValues()
        {
            var mockHttp = new MockHttpMessageHandler();

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
                                              ""sensor"": [
                                                  {
                                                      ""type"": 9,
                                                      ""value"": 21.3,
                                                      ""time"": 1556789036
                                                  },
                                                  {
                                                      ""type"": 13,
                                                      ""value"": 45.975,
                                                      ""time"": 1556787850
                                                  }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetZonesAndSensorValues();

            Assert.Equal(0, result.zones[0].ZoneID);
            Assert.Null(result.zones[0].sensor);

            Assert.Equal(32027, result.zones[2].ZoneID);
            Assert.Equal(9, result.zones[2].sensor[0].type);
            Assert.Equal(21.3, result.zones[2].sensor[0].value);
            Assert.Equal(1556789036, result.zones[2].sensor[0].time);
        }

        [Fact]
        public async Task TestGetMeteringCircuits()
        {
            var mockHttp = new MockHttpMessageHandler();

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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetMeteringCircuits();

            Assert.Equal("99999942f800000000000f0000deadbeef", result.DSMeters[0].DSUID);
            Assert.True(result.DSMeters[0].Capabilities[0].Metering);
            Assert.False(result.DSMeters[1].Capabilities[0].Metering);

            Assert.Single(result.FilteredMeterNames);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetCircuitZones();

            Assert.Equal("99999942f800000000000f0000deadbeef", result.DSMeters[0].DSUID);
            Assert.Equal(2, result.DSMeters[0].zones.Count);
            Assert.Equal(4, result.DSMeters[0].zones[0].ZoneID);
            Assert.Equal(32027, result.DSMeters[0].zones[1].ZoneID);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

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
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/metering/getValues")
                    .WithExactQueryString($"dsuid=99999942f800000000000f0000deadbeef&type=consumption&resolution=1&valueCount=2&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""meterID"": ""99999942f800000000000f0000deadbeef"",
                                      ""type"": ""consumption"",
                                      ""unit"": ""W"",
                                      ""resolution"": ""1"",
                                      ""values"": [
                                          [
                                              1556748119,
                                              6
                                          ],
                                          [
                                              1556748120,
                                              6
                                          ]
                                      ]
                                  },
                                  ""ok"": true
                              }");

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.GetEnergy(new DSUID("99999942f800000000000f0000deadbeef"), 1, 2);

            Assert.Equal("consumption", result.Type);
            Assert.Equal("W", result.Unit);
            Assert.Equal(1, result.Resolution);
            Assert.Equal(2, result.Values.Count);
            Assert.Equal(2, result.Values[0].Count);
            Assert.Equal(1556748119, result.Values[0][0]);
            Assert.Equal(6, result.Values[0][1]);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.Subscribe((SystemEventName)SystemEventName.EventType.CallScene, 42);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.Unsubscribe((SystemEventName)SystemEventName.EventType.CallScene, 42);
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

            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await dsApiClient.PollForEvents(42, 60000);

            Assert.Equal("callScene", result.events[0].name);
            Assert.Equal("2", result.events[0].properties.callOrigin);
            Assert.Equal("5", result.events[0].properties.sceneID);
            Assert.True(result.events[0].properties.forced);
            Assert.Equal("32027", result.events[0].properties.zoneID);
            Assert.Equal("5f4d6babc_dummy_unittest_token_83025a07162890c80a8b587bea589b8e2", result.events[0].properties.originToken);
            Assert.Equal("0000000000000000000000000000000000", result.events[0].properties.originDSUID);
            Assert.Equal(".zone(32027).group(1)", result.events[0].source.set);
            Assert.Equal(1, result.events[0].source.groupID);
            Assert.Equal(32027, result.events[0].source.zoneID);
            Assert.False(result.events[0].source.isApartment);
            Assert.True(result.events[0].source.isGroup);
            Assert.False(result.events[0].source.isDevice);
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
            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToMockProvider());

            await dsApiClient.RaiseEvent((SystemEventName)SystemEventName.EventType.CallScene,
                new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("mykey", "myval") });
        }

        [Fact]
        public async Task GenerateUnitTestRequestUris()
        {
            var mockHttp = new MockDigitalstromConnection.TestGenerationHttpMessageHandler();
            var dsApiClient = new DigitalstromWebserviceClient(mockHttp.AddAuthMock().ToTestGenerationProvider());

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

            try { await dsApiClient.CallScene(zoneKitchen, (Group)Group.Color.Yellow, (Scene)Scene.SceneCommand.Preset1, true); } catch { }
            UriForMethodName.Add("CallScene", mockHttp.LastCalledUri);

            try { await dsApiClient.GetReachableScenes(zoneKitchen, (Group)Group.Color.Yellow); } catch { }
            UriForMethodName.Add("GetReachableScenes", mockHttp.LastCalledUri);

            try { await dsApiClient.GetLastCalledScene(zoneKitchen, (Group)Group.Color.Yellow); } catch { }
            UriForMethodName.Add("GetLastCalledScene", mockHttp.LastCalledUri);

            try { await dsApiClient.GetZonesAndLastCalledScenes(); } catch { }
            UriForMethodName.Add("GetZonesAndLastCalledScenes", mockHttp.LastCalledUri);

            try { await dsApiClient.GetZonesAndSensorValues(); } catch { }
            UriForMethodName.Add("GetZonesAndSensorValues", mockHttp.LastCalledUri);

            try { await dsApiClient.GetMeteringCircuits(); } catch { }
            UriForMethodName.Add("GetMeteringCircuits", mockHttp.LastCalledUri);

            try { await dsApiClient.GetCircuitZones(); } catch { }
            UriForMethodName.Add("GetCircuitZones", mockHttp.LastCalledUri);

            try { await dsApiClient.GetTotalEnergy(1, 600); } catch { }
            UriForMethodName.Add("GetTotalEnergy", mockHttp.LastCalledUri);

            try { await dsApiClient.GetEnergy(new DSUID("99999942f800000000000f0000deadbeef"), 1, 600); } catch { }
            UriForMethodName.Add("GetEnergy", mockHttp.LastCalledUri);

            try { await dsApiClient.Subscribe((SystemEventName)SystemEventName.EventType.CallScene, 42); } catch { }
            UriForMethodName.Add("Subscribe", mockHttp.LastCalledUri);

            try { await dsApiClient.Unsubscribe((SystemEventName)SystemEventName.EventType.CallScene, 42); } catch { }
            UriForMethodName.Add("Unsubscribe", mockHttp.LastCalledUri);

            try { await dsApiClient.PollForEvents(42, 60000); } catch { }
            UriForMethodName.Add("PollForEvents", mockHttp.LastCalledUri);

            try { await dsApiClient.RaiseEvent((SystemEventName)SystemEventName.EventType.CallScene,
                new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("mykey", "myval") }); } catch { }
            UriForMethodName.Add("RaiseEvent", mockHttp.LastCalledUri);

            var allUris = string.Join("\n", UriForMethodName.Select(x => $"{x.Key}: {x.Value}"));
        }
    }
}