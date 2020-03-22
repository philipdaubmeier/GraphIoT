using RichardSzalay.MockHttp;
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
                    .Respond("application/json", @"{
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
        }

        [Fact]
        public async Task TestGetGateways()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}iot/v1/equipment/installations/{MockViessmannConnection.InstallationId}/gateways")
                    .Respond("application/json", @"{
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
        }

        [Fact]
        public async Task TestGetDevices()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}iot/v1/equipment/installations/{MockViessmannConnection.InstallationId}/gateways/{MockViessmannConnection.GatewayId}/devices")
                    .Respond("application/json", @"{
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
        }

        [Fact]
        public async Task TestGetFeatures()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockViessmannConnection.BaseUri}operational-data/v2/installations/{MockViessmannConnection.InstallationId}/gateways/{MockViessmannConnection.GatewayId}/devices/{MockViessmannConnection.DeviceId}/features?reduceHypermedia=true")
                    .Respond("application/json", @"{
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
                                                                           ""sun"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
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

            var result = await viessmannClient.GetFeatures(MockViessmannConnection.InstallationId, MockViessmannConnection.GatewayId, MockViessmannConnection.DeviceId);

            Assert.Equal(MockViessmannConnection.DeviceId.ToString(), result.First().DeviceId);
        }
    }
}