using PhilipDaubmeier.ViessmannClient.Model.Features;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.ViessmannClient.Tests
{
    public class ViessmannPlatformClientTest
    {
        [Fact]
        public async Task TestGetInstallations()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}iot/v1/equipment/installations")
                    .Respond("application/json",
                    @"{
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
                    }");

            using var viessmannClient = new ViessmannPlatformClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await viessmannClient.GetInstallations();

            Assert.Equal(MockViessmannConnection.InstallationId, result.First().LongId);
            Assert.Equal("Long test street", result.First().Address?.Street);
            Assert.Equal("1001", result.First().Address?.HouseNumber);
            Assert.Equal("12345", result.First().Address?.Zip);
            Assert.Equal("Unittest Town", result.First().Address?.City);
            Assert.Null(result.First().Address?.Region);
            Assert.Equal("de", result.First().Address?.Country);
            Assert.Null(result.First().Address?.PhoneNumber);
            Assert.Null(result.First().Address?.FaxNumber);
            Assert.Equal(37.377166d, result.First().Address?.Geolocation?.Latitude);
            Assert.Equal(-122.086966d, result.First().Address?.Geolocation?.Longitude);
            Assert.Equal("Europe/Berlin", result.First().Address?.Geolocation?.TimeZone);
            Assert.Equal("WorksProperly", result.First().AggregatedStatus);
            Assert.Null(result.First().ServicedBy);
            Assert.Null(result.First().HeatingType);
        }

        [Fact]
        public async Task TestGetGateways()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}iot/v1/equipment/installations/{MockViessmannConnection.InstallationId}/gateways")
                    .Respond("application/json",
                    @"{
                        ""data"": [
                            {
                                ""serial"": """ + MockViessmannConnection.GatewayId + @""",
                                ""version"": ""1.4.0.0"",
                                ""firmwareUpdateFailureCounter"": 0,
                                ""autoUpdate"": false,
                                ""createdAt"": ""2016-08-01T16:34:22.856Z"",
                                ""producedAt"": ""2016-08-01T14:40:28.000Z"",
                                ""lastStatusChangedAt"": ""2020-03-22T06:07:56.595Z"",
                                ""aggregatedStatus"": ""WorksProperly"",
                                ""targetRealm"": ""Genesis"",
                                ""gatewayType"": ""VitoconnectOptolink"",
                                ""installationId"": " + MockViessmannConnection.InstallationId + @",
                                ""registeredAt"": ""2017-01-29T18:19:44.813Z""
                            }
                        ],
                        ""cursor"": {
                            ""next"": """"
                        }
                    }");

            using var viessmannClient = new ViessmannPlatformClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await viessmannClient.GetGateways(MockViessmannConnection.InstallationId);

            Assert.Equal(MockViessmannConnection.GatewayId, result.First().LongId);
            Assert.Equal(MockViessmannConnection.GatewayId.ToString(), result.First().Serial);
            Assert.Equal("1.4.0.0", result.First().Version);
            Assert.False(result.First().AutoUpdate);
            Assert.Equal("WorksProperly", result.First().AggregatedStatus);
            Assert.Equal("Genesis", result.First().TargetRealm);
            Assert.Equal("VitoconnectOptolink", result.First().GatewayType);
            Assert.Equal(MockViessmannConnection.InstallationId, (long?)result.First().InstallationId);
        }

        [Fact]
        public async Task TestGetDevices()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}iot/v1/equipment/installations/{MockViessmannConnection.InstallationId}/gateways/{MockViessmannConnection.GatewayId}/devices")
                    .Respond("application/json",
                    @"{
                        ""data"": [
                            {
                                ""gatewaySerial"": """ + MockViessmannConnection.GatewayId + @""",
                                ""id"": """ + MockViessmannConnection.DeviceId + @""",
                                ""boilerSerial"": ""777555888999"",
                                ""boilerSerialEditor"": ""DeviceCommunication"",
                                ""bmuSerial"": ""000999888777"",
                                ""bmuSerialEditor"": ""DeviceCommunication"",
                                ""createdAt"": ""2018-06-08T00:08:10.199Z"",
                                ""editedAt"": ""2019-11-08T20:53:43.470Z"",
                                ""modelId"": ""VPlusHO1_40"",
                                ""status"": ""Online"",
                                ""deviceType"": ""heating""
                            }
                        ]
                    }");

            using var viessmannClient = new ViessmannPlatformClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await viessmannClient.GetDevices(MockViessmannConnection.InstallationId, MockViessmannConnection.GatewayId);

            Assert.Equal(MockViessmannConnection.DeviceId, result.First().LongId);
            Assert.Equal(MockViessmannConnection.GatewayId.ToString(), result.First().GatewaySerial);
            Assert.Equal(MockViessmannConnection.DeviceId.ToString(), result.First().Id);
            Assert.Equal("777555888999", result.First().BoilerSerial);
            Assert.Equal("DeviceCommunication", result.First().BoilerSerialEditor);
            Assert.Equal("000999888777", result.First().BmuSerial);
            Assert.Equal("DeviceCommunication", result.First().BmuSerialEditor);
            Assert.Equal("VPlusHO1_40", result.First().ModelId);
            Assert.Equal("Online", result.First().Status);
            Assert.Equal("heating", result.First().DeviceType);
        }

        [Fact]
        public async Task TestGetGatewayFeatures()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}operational-data/v2/installations/{MockViessmannConnection.InstallationId}/gateways/{MockViessmannConnection.GatewayId}/features?reduceHypermedia=true")
                    .Respond("application/json",
                    @"{
                        ""features"": [
                            {
                                ""isEnabled"": true,
                                ""isReady"": true,
                                ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                ""feature"": ""gateway.bmuconnection"",
                                ""timestamp"": ""2020-03-23T19:54:09.825Z"",
                                ""properties"": {
                                    ""status"": {
                                        ""type"": ""string"",
                                        ""value"": ""OK""
                                    }
                                },
                                ""commands"": {},
                                ""components"": []
                            },
                            {
                                ""isEnabled"": true,
                                ""isReady"": true,
                                ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                ""feature"": ""gateway.devices"",
                                ""timestamp"": ""2020-03-23T19:54:09.825Z"",
                                ""properties"": {
                                    ""devices"": {
                                        ""type"": ""DeviceList"",
                                        ""value"": [
                                            {
                                                ""id"": """ + MockViessmannConnection.DeviceId + @""",
                                                ""type"": ""heating"",
                                                ""status"": ""online"",
                                                ""fingerprint"": ""vv:12,jh:b9,gh:777,ijk:12345"",
                                                ""modelId"": ""VPlusHO1_40"",
                                                ""modelVersion"": ""19.1.22.1"",
                                                ""name"": ""VT 200 (HO1A / HO1B)""
                                            }
                                        ]
                                    }
                                },
                                ""commands"": {},
                                ""components"": []
                            },
                            {
                                ""isEnabled"": true,
                                ""isReady"": true,
                                ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                ""feature"": ""gateway.firmware"",
                                ""timestamp"": ""2020-03-23T19:54:09.825Z"",
                                ""properties"": {
                                    ""version"": {
                                        ""type"": ""string"",
                                        ""value"": ""1.4.0""
                                    },
                                    ""updateStatus"": {
                                        ""type"": ""string"",
                                        ""value"": ""idle""
                                    }
                                },
                                ""commands"": {},
                                ""components"": []
                            },
                            {
                                ""isEnabled"": true,
                                ""isReady"": true,
                                ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                ""feature"": ""gateway.status"",
                                ""timestamp"": ""2020-03-23T19:54:09.825Z"",
                                ""properties"": {
                                    ""online"": {
                                        ""type"": ""boolean"",
                                        ""value"": true
                                    }
                                },
                                ""commands"": {},
                                ""components"": []
                            },
                            {
                                ""isEnabled"": true,
                                ""isReady"": true,
                                ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                ""feature"": ""gateway.wifi"",
                                ""timestamp"": ""2020-03-23T19:54:09.825Z"",
                                ""properties"": {
                                    ""strength"": {
                                        ""type"": ""number"",
                                        ""value"": -66
                                    },
                                    ""channel"": {
                                        ""type"": ""number"",
                                        ""value"": 7
                                    }
                                },
                                ""commands"": {},
                                ""components"": []
                            }
                        ]
                    }");

            using var viessmannClient = new ViessmannPlatformClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await viessmannClient.GetGatewayFeatures(MockViessmannConnection.InstallationId, MockViessmannConnection.GatewayId);

            Assert.Equal(MockViessmannConnection.GatewayId.ToString(), result.First().GatewayId);
            Assert.Equal(5, result.Count());

            var feature1 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.GatewayStatus))?.Properties;
            var feature2 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.GatewayWifi))?.Properties;
            var feature3 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.GatewayBmuconnection))?.Properties;
            var feature4 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.GatewayDevices))?.Properties;
            var feature5 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.GatewayFirmware))?.Properties;

            Assert.Equal("boolean", feature1?.Online?.Type);
            Assert.True(feature1?.Online?.Value);

            Assert.Equal("number", feature2?.Strength?.Type);
            Assert.Equal(-66, feature2?.Strength?.Value);
            Assert.Equal("number", feature2?.Channel?.Type);
            Assert.Equal(7, feature2?.Channel?.Value);

            Assert.Equal("string", feature3?.Status?.Type);
            Assert.Equal("OK", feature3?.Status?.Value);

            Assert.Equal("DeviceList", feature4?.Devices?.Type);
            Assert.Equal(MockViessmannConnection.DeviceId, feature4?.Devices?.Value?.FirstOrDefault()?.LongId);
            Assert.Equal("heating", feature4?.Devices?.Value?.FirstOrDefault()?.Type);
            Assert.Equal("online", feature4?.Devices?.Value?.FirstOrDefault()?.Status);
            Assert.Equal("vv:12,jh:b9,gh:777,ijk:12345", feature4?.Devices?.Value?.FirstOrDefault()?.Fingerprint);
            Assert.Equal("VPlusHO1_40", feature4?.Devices?.Value?.FirstOrDefault()?.ModelId);
            Assert.Equal("19.1.22.1", feature4?.Devices?.Value?.FirstOrDefault()?.ModelVersion);
            Assert.Equal("VT 200 (HO1A / HO1B)", feature4?.Devices?.Value?.FirstOrDefault()?.Name);

            Assert.Equal("string", feature5?.Version?.Type);
            Assert.Equal("1.4.0", feature5?.Version?.Value);
            Assert.Equal("string", feature5?.UpdateStatus?.Type);
            Assert.Equal("idle", feature5?.UpdateStatus?.Value);
        }

        [Fact]
        public async Task TestGetDeviceFeatures()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}operational-data/v2/installations/{MockViessmannConnection.InstallationId}/gateways/{MockViessmannConnection.GatewayId}/devices/{MockViessmannConnection.DeviceId}/features?reduceHypermedia=true")
                    .Respond("application/json",
                    @"{
                          ""features"": [
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.boiler.temperature"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.688Z"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 47.6
                                      }
                                  },
                                  ""commands"": {},
                                  ""components"": []
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.circuits"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.688Z"",
                                  ""properties"": {
                                      ""enabled"": {
                                          ""type"": ""array"",
                                          ""value"": [ ""0"", ""1"" ]
                                      }
                                  },
                                  ""commands"": {},
                                  ""components"": [ ""0"", ""1"", ""2"" ]
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.circuits.0.circulation.pump"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.690Z"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""on""
                                      }
                                  },
                                  ""commands"": {},
                                  ""components"": []
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.circuits.0.heating.curve"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.695Z"",
                                  ""properties"": {
                                      ""shift"": {
                                          ""type"": ""number"",
                                          ""value"": 5
                                      },
                                      ""slope"": {
                                          ""type"": ""number"",
                                          ""value"": 0.9
                                      }
                                  },
                                  ""commands"": {
                                      ""setCurve"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.heating.curve/setCurve"",
                                          ""name"": ""setCurve"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""slope"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": { ""min"": 0.2, ""max"": 3.5, ""stepping"": 0.1 }
                                              },
                                              ""shift"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": { ""min"": -13, ""max"": 40, ""stepping"": 1 }
                                              }
                                          }
                                      }
                                  },
                                  ""components"": []
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.dhw.schedule"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.736Z"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""entries"": {
                                          ""type"": ""Schedule"",
                                          ""value"": {
                                              ""mon"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""tue"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""wed"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""thu"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""fri"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sat"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sun"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ]
                                          }
                                      }
                                  },
                                  ""commands"": {
                                      ""setSchedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.dhw.schedule/setSchedule"",
                                          ""name"": ""setSchedule"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""newSchedule"": {
                                                  ""required"": true,
                                                  ""type"": ""Schedule"",
                                                  ""constraints"": {
                                                      ""maxEntries"": 4,
                                                      ""resolution"": 10,
                                                      ""modes"": [
                                                          ""on""
                                                      ],
                                                      ""defaultMode"": ""off""
                                                  }
                                              }
                                          }
                                      }
                                  },
                                  ""components"": []
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.dhw.temperature.main"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.740Z"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 50
                                      }
                                  },
                                  ""commands"": {
                                      ""setTargetTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.dhw.temperature.main/setTargetTemperature"",
                                          ""name"": ""setTargetTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""temperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 10,
                                                      ""max"": 60,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  },
                                  ""components"": []
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.sensors.temperature.outside"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.741Z"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""connected""
                                      },
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 6
                                      }
                                  },
                                  ""commands"": {},
                                  ""components"": []
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.solar"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.742Z"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      }
                                  },
                                  ""commands"": {},
                                  ""components"": [ ""statistics"", ""sensors"", ""rechargeSuppression"" ]
                              },
                              {
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""gatewayId"": """ + MockViessmannConnection.GatewayId + @""",
                                  ""feature"": ""heating.solar.power.production"",
                                  ""deviceId"": """ + MockViessmannConnection.DeviceId + @""",
                                  ""timestamp"": ""2020-03-23T12:19:28.742Z"",
                                  ""properties"": {
                                      ""day"": {
                                          ""type"": ""array"",
                                          ""value"": [ 9.914, 11.543 ]
                                      },
                                      ""week"": { ""type"": ""array"", ""value"": [] },
                                      ""month"": { ""type"": ""array"", ""value"": [] },
                                      ""year"": { ""type"": ""array"", ""value"": [] },
                                      ""unit"": {
                                          ""type"": ""string"",
                                          ""value"": ""kilowattHour""
                                      }
                                  },
                                  ""commands"": {},
                                  ""components"": []
                              }
                          ]
                      }");

            using var viessmannClient = new ViessmannPlatformClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await viessmannClient.GetDeviceFeatures(MockViessmannConnection.InstallationId, MockViessmannConnection.GatewayId, MockViessmannConnection.DeviceId);

            Assert.Equal(MockViessmannConnection.DeviceId.ToString(), result.First().DeviceId);
            Assert.Equal(9, result.Count());

            var feature1 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingBoilerTemperature))?.Properties;
            var feature2 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingCircuits))?.Properties;
            var feature3 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingCircuitsCirculationPump))?.Properties;
            var feature4 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingCircuitsHeatingCurve))?.Properties;
            var feature5 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingDhwSchedule))?.Properties;
            var feature6 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingDhwTemperatureMain))?.Properties;
            var feature7 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingSensorsTemperatureOutside))?.Properties;
            var feature8 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingSolar))?.Properties;
            var feature9 = result.GetRawFeatureByName(new FeatureName(FeatureName.Name.HeatingSolarPowerProduction))?.Properties;

            Assert.Equal("number", feature1?.Value?.Type);
            Assert.Equal(47.6, feature1?.Value?.Value);

            Assert.Equal("array", feature2?.Enabled?.Type);
            Assert.Equal(new[] { "0", "1" }.ToList<string?>(), feature2?.Enabled?.Value);

            Assert.Equal("string", feature3?.Status?.Type);
            Assert.Equal("on", feature3?.Status?.Value);

            Assert.Equal("number", feature4?.Shift?.Type);
            Assert.Equal(5m, feature4?.Shift?.Value);
            Assert.Equal("number", feature4?.Slope?.Type);
            Assert.Equal(0.9m, feature4?.Slope?.Value);

            Assert.Equal("boolean", feature5?.Active?.Type);
            Assert.True(feature5?.Active?.Value);
            Assert.Equal("Schedule", feature5?.Entries?.Type);
            Assert.Equal("00:00", feature5?.Entries?.Value?.Mon?.First()?.Start);
            Assert.Equal("24:00", feature5?.Entries?.Value?.Mon?.First()?.End);
            Assert.Equal("on", feature5?.Entries?.Value?.Mon?.First()?.Mode);
            Assert.Equal(0, feature5?.Entries?.Value?.Mon?.First()?.Position);

            Assert.Equal("number", feature6?.Value?.Type);
            Assert.Equal(50L, feature6?.Value?.Value);

            Assert.Equal("string", feature7?.Status?.Type);
            Assert.Equal("connected", feature7?.Status?.Value);
            Assert.Equal("number", feature7?.Value?.Type);
            Assert.Equal(6L, feature7?.Value?.Value);

            Assert.Equal("boolean", feature8?.Active?.Type);
            Assert.True(feature8?.Active?.Value);

            Assert.Equal("array", feature9?.Day?.Type);
            Assert.Equal(new[] { 9.914d, 11.543d }.ToList(), feature9?.Day?.Value.ToList());
            Assert.Equal(new List<double>(), feature9?.Week?.Value.ToList());
            Assert.Equal(new List<double>(), feature9?.Month?.Value.ToList());
            Assert.Equal(new List<double>(), feature9?.Year?.Value.ToList());
            Assert.Equal("string", feature9?.Unit?.Type);
            Assert.Equal("kilowattHour", feature9?.Unit?.Value);
        }

        [Fact]
        public async Task TestGetDeviceFeaturePropertyMethods()
        {
            var mockHttp = new MockHttpMessageHandler();

            using var viessmannClient = new ViessmannPlatformClient(mockHttp
                .AddAuthMock()
                .AddAllDeviceFeatures()
                .ToMockProvider());

            var result = await viessmannClient.GetDeviceFeatures(MockViessmannConnection.InstallationId, MockViessmannConnection.GatewayId, MockViessmannConnection.DeviceId);

            Assert.Equal(157, result.Count());

            Assert.Equal("2020-01-01T00:00:00.000Z", result.GetDeviceMessagesLogbook().First().Timestamp);
            Assert.Equal("SECONDARY_PUMP1", result.GetDeviceMessagesLogbook().First().Actor);
            Assert.Equal(0L, result.GetDeviceMessagesLogbook().First().Status);
            Assert.Equal("Inverter_CPU_error", result.GetDeviceMessagesLogbook().First().Event);
            Assert.Equal("TPM_SC1", result.GetDeviceMessagesLogbook().First().StateMachine);
            Assert.Equal(120, result.GetDeviceMessagesLogbook().First().AdditionalInfo);
            Assert.Equal("3344556677", result.GetHeatingBoilerSerial());
            Assert.False(result.IsHeatingBoilerSensorsTemperatureCommonSupplyConnected());
            Assert.Equal(double.NaN, result.GetHeatingBoilerSensorsTemperatureCommonSupply());
            Assert.True(result.IsHeatingBoilerSensorsTemperatureMainConnected());
            Assert.Equal(47, result.GetHeatingBoilerSensorsTemperatureMain());
            Assert.Equal(47.4, result.GetHeatingBoilerTemperature());
            Assert.False(result.IsHeatingBurnerActive());
            Assert.True(result.IsGetHeatingBurnerAutomaticStatusOk());
            Assert.Equal(0, result.GetHeatingBurnerAutomaticErrorCode());
            Assert.Equal(0, result.GetHeatingBurnerModulation());
            Assert.Equal(9876.12m, result.GetHeatingBurnerStatisticsHours());
            Assert.Equal(44321, result.GetHeatingBurnerStatisticsStarts());
            Assert.Equal(new List<FeatureName.Circuit>() { FeatureName.Circuit.Circuit0, FeatureName.Circuit.Circuit1 }, result.GetHeatingCircuits());
            Assert.True(result.IsHeatingCircuitsCircuitActive(FeatureName.Circuit.Circuit0));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsCircuitName(FeatureName.Circuit.Circuit0));
            Assert.True(result.IsHeatingCircuitsCirculationPumpOn(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsFrostprotectionOn(FeatureName.Circuit.Circuit0));
            Assert.Equal(5, result.GetHeatingCircuitsHeatingCurveShift(FeatureName.Circuit.Circuit0));
            Assert.Equal(0.9m, result.GetHeatingCircuitsHeatingCurveSlope(FeatureName.Circuit.Circuit0));
            Assert.True(result.IsHeatingCircuitsHeatingScheduleActive(FeatureName.Circuit.Circuit0));
            Assert.NotEmpty(result.GetHeatingCircuitsHeatingSchedule(FeatureName.Circuit.Circuit0).Mon);
            Assert.Equal("dhwAndHeating", result.GetHeatingCircuitsOperatingModesActive(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingModesDhwActive(FeatureName.Circuit.Circuit0));
            Assert.True(result.IsHeatingCircuitsOperatingModesDhwAndHeatingActive(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingModesForcedNormalActive(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingModesForcedReducedActive(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingModesStandbyActive(FeatureName.Circuit.Circuit0));
            Assert.Equal("normal", result.GetHeatingCircuitsOperatingProgramsActive(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsComfortActive(FeatureName.Circuit.Circuit0));
            Assert.Equal(20, result.GetHeatingCircuitsOperatingProgramsComfortTemperature(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsEcoActive(FeatureName.Circuit.Circuit0));
            Assert.Equal(23, result.GetHeatingCircuitsOperatingProgramsEcoTemperature(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsExternalActive(FeatureName.Circuit.Circuit0));
            Assert.Equal(0, result.GetHeatingCircuitsOperatingProgramsExternalTemperature(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsHolidayActive(FeatureName.Circuit.Circuit0));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingProgramsHolidayStart(FeatureName.Circuit.Circuit0));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingProgramsHolidayEnd(FeatureName.Circuit.Circuit0));
            Assert.True(result.IsHeatingCircuitsOperatingProgramsNormalActive(FeatureName.Circuit.Circuit0));
            Assert.Equal(23, result.GetHeatingCircuitsOperatingProgramsNormalTemperature(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsReducedActive(FeatureName.Circuit.Circuit0));
            Assert.Equal(10, result.GetHeatingCircuitsOperatingProgramsReducedTemperature(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsStandbyActive(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureConnected(FeatureName.Circuit.Circuit0));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperature(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureRoomConnected(FeatureName.Circuit.Circuit0));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperatureRoom(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureSupplyConnected(FeatureName.Circuit.Circuit0));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperatureSupply(FeatureName.Circuit.Circuit0));
            Assert.False(result.IsHeatingCircuitsGeofencingActive(FeatureName.Circuit.Circuit0));
            Assert.Equal("home", result.GetHeatingCircuitsGeofencingStatus(FeatureName.Circuit.Circuit0));
            Assert.True(result.IsHeatingCircuitsCircuitActive(FeatureName.Circuit.Circuit1));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsCircuitName(FeatureName.Circuit.Circuit1));
            Assert.True(result.IsHeatingCircuitsCirculationPumpOn(FeatureName.Circuit.Circuit1));
            Assert.True(result.IsHeatingCircuitsFrostprotectionOn(FeatureName.Circuit.Circuit1));
            Assert.Equal(0, result.GetHeatingCircuitsHeatingCurveShift(FeatureName.Circuit.Circuit1));
            Assert.Equal(0.5m, result.GetHeatingCircuitsHeatingCurveSlope(FeatureName.Circuit.Circuit1));
            Assert.True(result.IsHeatingCircuitsHeatingScheduleActive(FeatureName.Circuit.Circuit1));
            Assert.NotEmpty(result.GetHeatingCircuitsHeatingSchedule(FeatureName.Circuit.Circuit1).Mon);
            Assert.Equal("dhwAndHeating", result.GetHeatingCircuitsOperatingModesActive(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingModesDhwActive(FeatureName.Circuit.Circuit1));
            Assert.True(result.IsHeatingCircuitsOperatingModesDhwAndHeatingActive(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingModesForcedNormalActive(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingModesForcedReducedActive(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingModesStandbyActive(FeatureName.Circuit.Circuit1));
            Assert.Equal("normal", result.GetHeatingCircuitsOperatingProgramsActive(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsComfortActive(FeatureName.Circuit.Circuit1));
            Assert.Equal(24, result.GetHeatingCircuitsOperatingProgramsComfortTemperature(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsEcoActive(FeatureName.Circuit.Circuit1));
            Assert.Equal(21, result.GetHeatingCircuitsOperatingProgramsEcoTemperature(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsExternalActive(FeatureName.Circuit.Circuit1));
            Assert.Equal(0, result.GetHeatingCircuitsOperatingProgramsExternalTemperature(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsHolidayActive(FeatureName.Circuit.Circuit1));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingProgramsHolidayStart(FeatureName.Circuit.Circuit1));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingProgramsHolidayEnd(FeatureName.Circuit.Circuit1));
            Assert.True(result.IsHeatingCircuitsOperatingProgramsNormalActive(FeatureName.Circuit.Circuit1));
            Assert.Equal(21, result.GetHeatingCircuitsOperatingProgramsNormalTemperature(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsReducedActive(FeatureName.Circuit.Circuit1));
            Assert.Equal(12, result.GetHeatingCircuitsOperatingProgramsReducedTemperature(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsStandbyActive(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureConnected(FeatureName.Circuit.Circuit1));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperature(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureRoomConnected(FeatureName.Circuit.Circuit1));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperatureRoom(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureSupplyConnected(FeatureName.Circuit.Circuit1));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperatureSupply(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsGeofencingActive(FeatureName.Circuit.Circuit1));
            Assert.Equal("home", result.GetHeatingCircuitsGeofencingStatus(FeatureName.Circuit.Circuit1));
            Assert.False(result.IsHeatingCircuitsCircuitActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsCircuitName(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsCirculationPumpOn(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsFrostprotectionOn(FeatureName.Circuit.Circuit2));
            Assert.Equal(0, result.GetHeatingCircuitsHeatingCurveShift(FeatureName.Circuit.Circuit2));
            Assert.Equal(0, result.GetHeatingCircuitsHeatingCurveSlope(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsHeatingScheduleActive(FeatureName.Circuit.Circuit2));
            Assert.Null(result.GetHeatingCircuitsHeatingSchedule(FeatureName.Circuit.Circuit2).Mon);
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingModesActive(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingModesDhwActive(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingModesDhwAndHeatingActive(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingModesForcedNormalActive(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingModesForcedReducedActive(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingModesStandbyActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingProgramsActive(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsComfortActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(0, result.GetHeatingCircuitsOperatingProgramsComfortTemperature(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsEcoActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(0, result.GetHeatingCircuitsOperatingProgramsEcoTemperature(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsExternalActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(0, result.GetHeatingCircuitsOperatingProgramsExternalTemperature(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsHolidayActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingProgramsHolidayStart(FeatureName.Circuit.Circuit2));
            Assert.Equal(string.Empty, result.GetHeatingCircuitsOperatingProgramsHolidayEnd(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsNormalActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(0, result.GetHeatingCircuitsOperatingProgramsNormalTemperature(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsReducedActive(FeatureName.Circuit.Circuit2));
            Assert.Equal(0, result.GetHeatingCircuitsOperatingProgramsReducedTemperature(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsOperatingProgramsStandbyActive(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureConnected(FeatureName.Circuit.Circuit2));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperature(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureRoomConnected(FeatureName.Circuit.Circuit2));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperatureRoom(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsSensorsTemperatureSupplyConnected(FeatureName.Circuit.Circuit2));
            Assert.Equal(double.NaN, result.GetHeatingCircuitsSensorsTemperatureSupply(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingCircuitsGeofencingActive(FeatureName.Circuit.Circuit2));
            Assert.Equal("home", result.GetHeatingCircuitsGeofencingStatus(FeatureName.Circuit.Circuit2));
            Assert.False(result.IsHeatingConfigurationMultiFamilyHouseActive());
            Assert.Equal("2020-09-15T06:38:01.000Z", result.GetHeatingCoolingCircuitMessages(FeatureName.Circuit.Circuit0).First().Timestamp);
            Assert.Equal("49", result.GetHeatingCoolingCircuitMessages(FeatureName.Circuit.Circuit0).First().ErrorCode);
            Assert.Equal("inactive", result.GetHeatingCoolingCircuitMessages(FeatureName.Circuit.Circuit0).First().Status);
            Assert.Equal(39, result.GetHeatingCoolingCircuitMessages(FeatureName.Circuit.Circuit0).First().Count);
            Assert.Equal("info", result.GetHeatingCoolingCircuitMessages(FeatureName.Circuit.Circuit0).First().Priority);
            Assert.Equal("777555888999", result.GetHeatingControllerSerial());
            Assert.Equal(52, result.GetHeatingDeviceTimeOffset());
            Assert.True(result.IsHeatingDhwActive());
            Assert.False(result.IsHeatingDhwChargingActive());
            Assert.False(result.IsHeatingDhwPumpsCirculationOn());
            Assert.True(result.IsHeatingDhwPumpsCirculationScheduleActive());
            Assert.NotEmpty(result.GetHeatingDhwPumpsCirculationSchedule().Mon);
            Assert.False(result.IsHeatingDhwPumpsPrimaryOn());
            Assert.True(result.IsHeatingDhwScheduleActive());
            Assert.NotEmpty(result.GetHeatingDhwSchedule().Mon);
            Assert.True(result.IsHeatingDhwSensorsTemperatureHotWaterStorageConnected());
            Assert.Equal(55.4, result.GetHeatingDhwSensorsTemperatureHotWaterStorage());
            Assert.False(result.IsHeatingDhwSensorsTemperatureOutletConnected());
            Assert.Equal(double.NaN, result.GetHeatingDhwSensorsTemperatureOutlet());
            Assert.Equal(50, result.GetHeatingDhwTemperature());
            Assert.Equal(50, result.GetHeatingDhwTemperatureMain());
            Assert.True(result.IsHeatingSensorsTemperatureOutsideConnected());
            Assert.Equal(7.3, result.GetHeatingSensorsTemperatureOutside());
            Assert.False(result.GetHeatingServiceDue());
            Assert.Equal(0, result.GetHeatingServiceIntervalMonths());
            Assert.Equal(0, result.GetHeatingActiveMonthSinceLastService());
            Assert.Equal(string.Empty, result.GetHeatingLastService());
            Assert.True(result.IsHeatingSolarActive());
            Assert.Equal(10698, result.GetHeatingSolarPowerProductionWhToday());
            Assert.Equal(new List<double>() { 10.698, 7.193, 6.543, 0, 10.172, 21.322, 17.562, 6.99 }, result.GetHeatingSolarPowerProduction());
            Assert.Equal(new List<double>(), result.GetHeatingSolarPowerProduction(Resolution.Week));
            Assert.Equal(new List<double>(), result.GetHeatingSolarPowerProduction(Resolution.Month));
            Assert.Equal(new List<double>(), result.GetHeatingSolarPowerProduction(Resolution.Year));
            Assert.Equal("kilowattHour", result.GetHeatingSolarPowerProductionUnit());
            Assert.True(result.IsHeatingSolarPumpsCircuitOn());
            Assert.Equal(11085, result.GetHeatingSolarStatisticsHours());
            Assert.True(result.IsHeatingSolarSensorsTemperatureDhwConnected());
            Assert.Equal(41.4, result.GetHeatingSolarSensorsTemperatureDhw());
            Assert.True(result.IsHeatingSolarSensorsTemperatureCollectorConnected());
            Assert.Equal(50.7, result.GetHeatingSolarSensorsTemperatureCollector());
            Assert.Equal(23456, result.GetHeatingSolarPowerCumulativeProduced());
            Assert.True(result.IsHeatingSolarRechargeSuppressionOn());
        }
    }
}