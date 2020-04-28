using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public class WeConnectPortalClientTest
    {
        [Fact]
        public async Task TestGetEManager()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/emanager/get-emanager")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""EManager"":
                        {
                            ""rbc"":
                            {
                                ""status"":
                                {
                                    ""batteryPercentage"": 100,
                                    ""chargingState"": ""OFF"",
                                    ""chargingRemaningHour"": """",
                                    ""chargingRemaningMinute"": """",
                                    ""chargingReason"": ""INVALID"",
                                    ""pluginState"": ""CONNECTED"",
                                    ""lockState"": ""LOCKED"",
                                    ""extPowerSupplyState"": ""AVAILABLE"",
                                    ""range"": ""7"",
                                    ""electricRange"": 265,
                                    ""combustionRange"": null,
                                    ""combinedRange"": 265,
                                    ""rlzeUp"": false
                                },
                                ""settings"":
                                {
                                    ""chargerMaxCurrent"": 16,
                                    ""maxAmpere"": 32,
                                    ""maxCurrentReduced"": false
                                }
                            },
                            ""rpc"":
                            {
                                ""status"":
                                {
                                    ""climatisationState"": ""OFF"",
                                    ""climatisationRemaningTime"": 30,
                                    ""windowHeatingStateFront"": null,
                                    ""windowHeatingStateRear"": null,
                                    ""climatisationReason"": null,
                                    ""windowHeatingAvailable"": true
                                },
                                ""settings"":
                                {
                                    ""targetTemperature"": ""20.5"",
                                    ""climatisationWithoutHVPower"": true,
                                    ""electric"": true
                                },
                                ""climaterActionState"": ""AVAILABLE"",
                                ""auAvailable"": false
                            },
                            ""rdt"":
                            {
                                ""status"":
                                {
                                    ""timers"":
                                    [
                                        {
                                            ""timerId"": 1,
                                            ""timerProfileId"": 1,
                                            ""timerStatus"": ""NOT_EXPIRED"",
                                            ""timerChargeScheduleStatus"": ""IDLE"",
                                            ""timerClimateScheduleStatus"": ""IDLE"",
                                            ""timerExpStatusTimestamp"": ""28.04.2020"",
                                            ""timerProgrammedStatus"": ""NOT_PROGRAMMED"",
                                            ""schedule"":
                                            {
                                                ""type"": 2,
                                                ""start"": {""hours"":12,""minutes"":0},
                                                ""end"": {""hours"":null,""minutes"":null},
                                                ""index"": null,
                                                ""daypicker"": [""Y"",""Y"",""Y"",""Y"",""Y"",""Y"",""Y""],
                                                ""startDateActive"": ""28.04.2020"",
                                                ""endDateActive"": null
                                            },
                                            ""startDateActive"": ""28.04.2020"",
                                            ""timeRangeActive"": ""12:00""
                                        }
                                    ],
                                    ""profiles"":
                                    [
                                        {
                                            ""profileId"": 1,
                                            ""profileName"": ""Standard"",
                                            ""timeStamp"": ""28.04.2020"",
                                            ""charging"": true,
                                            ""climatisation"": false,
                                            ""targetChargeLevel"": 100,
                                            ""nightRateActive"": false,
                                            ""nightRateTimeStart"": ""23:00"",
                                            ""nightRateTimeEnd"": ""23:00"",
                                            ""chargeMaxCurrent"": 16,
                                            ""heaterSource"": ""ELECTRIC""
                                        }
                                    ]
                                },
                                ""settings"":
                                {
                                    ""minChargeLimit"": 30,
                                    ""lowerLimitMax"": 100
                                },
                                ""auxHeatingAllowed"": false,
                                ""auxHeatingEnabled"": false
                            },
                            ""actionPending"": false,
                            ""rdtAvailable"": true
                        }
                    }");

            var client = new WeConnectPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await client.GetEManager();

            Assert.Equal(100, result.Rbc.Status?.BatteryPercentage);
            Assert.Equal("OFF", result.Rbc.Status?.ChargingState);
            Assert.Equal("", result.Rbc.Status?.ChargingRemaningHour);
            Assert.Equal("", result.Rbc.Status?.ChargingRemaningMinute);
            Assert.Equal("INVALID", result.Rbc.Status?.ChargingReason);
            Assert.Equal("CONNECTED", result.Rbc.Status?.PluginState);
            Assert.Equal("LOCKED", result.Rbc.Status?.LockState);
            Assert.Equal("AVAILABLE", result.Rbc.Status?.ExtPowerSupplyState);
            Assert.Equal("7", result.Rbc.Status?.Range);
            Assert.Equal(265, result.Rbc.Status?.ElectricRange);
            Assert.Null(result.Rbc.Status?.CombustionRange);
            Assert.Equal(265, result.Rbc.Status?.CombinedRange);
            Assert.False(result.Rbc.Status?.RlzeUp);

            Assert.Equal(16, result.Rbc.Settings?.ChargerMaxCurrent);
            Assert.Equal(32, result.Rbc.Settings?.MaxAmpere);
            Assert.False(result.Rbc.Settings?.MaxCurrentReduced);

            Assert.Equal("OFF", result.Rpc.Status?.ClimatisationState);
            Assert.Equal(30, result.Rpc.Status?.ClimatisationRemaningTime);
            Assert.Null(result.Rpc.Status?.WindowHeatingStateFront);
            Assert.Null(result.Rpc.Status?.WindowHeatingStateRear);
            Assert.Null(result.Rpc.Status?.ClimatisationReason);
            Assert.True(result.Rpc.Status?.WindowHeatingAvailable);

            Assert.Equal("20.5", result.Rpc.Settings?.TargetTemperature);
            Assert.True(result.Rpc.Settings?.ClimatisationWithoutHVPower);
            Assert.True(result.Rpc.Settings?.Electric);
            Assert.Equal("AVAILABLE", result.Rpc.ClimaterActionState);
            Assert.False(result.Rpc.AuAvailable);

            Assert.Equal(1, result.Rdt.Status?.Timers[0].TimerId);
            Assert.Equal(1, result.Rdt.Status?.Timers[0].TimerProfileId);
            Assert.Equal("NOT_EXPIRED", result.Rdt.Status?.Timers[0].TimerStatus);
            Assert.Equal("IDLE", result.Rdt.Status?.Timers[0].TimerChargeScheduleStatus);
            Assert.Equal("IDLE", result.Rdt.Status?.Timers[0].TimerClimateScheduleStatus);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Timers[0].TimerExpStatusTimestamp);
            Assert.Equal("NOT_PROGRAMMED", result.Rdt.Status?.Timers[0].TimerProgrammedStatus);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Timers[0].StartDateActive);
            Assert.Equal("12:00", result.Rdt.Status?.Timers[0].TimeRangeActive);

            Assert.Equal(2, result.Rdt.Status?.Timers[0].Schedule.Type);
            Assert.Equal(12, result.Rdt.Status?.Timers[0].Schedule.Start.Hours);
            Assert.Equal(0, result.Rdt.Status?.Timers[0].Schedule.Start.Minutes);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.End.Hours);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.End.Minutes);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.Index);
            Assert.Equal("Y", result.Rdt.Status?.Timers[0].Schedule.Daypicker[0]);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Timers[0].Schedule.StartDateActive);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.EndDateActive);

            Assert.Equal(1, result.Rdt.Status?.Profiles[0].ProfileId);
            Assert.Equal("Standard", result.Rdt.Status?.Profiles[0].ProfileName);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Profiles[0].TimeStamp);
            Assert.True(result.Rdt.Status?.Profiles[0].Charging);
            Assert.False(result.Rdt.Status?.Profiles[0].Climatisation);
            Assert.Equal(100, result.Rdt.Status?.Profiles[0].TargetChargeLevel);
            Assert.False(result.Rdt.Status?.Profiles[0].NightRateActive);
            Assert.Equal("23:00", result.Rdt.Status?.Profiles[0].NightRateTimeStart);
            Assert.Equal("23:00", result.Rdt.Status?.Profiles[0].NightRateTimeEnd);
            Assert.Equal(16, result.Rdt.Status?.Profiles[0].ChargeMaxCurrent);
            Assert.Equal("ELECTRIC", result.Rdt.Status?.Profiles[0].HeaterSource);

            Assert.Equal(30, result.Rdt.Settings?.MinChargeLimit);
            Assert.Equal(100, result.Rdt.Settings?.LowerLimitMax);

            Assert.False(result.Rdt.AuxHeatingAllowed);
            Assert.False(result.Rdt.AuxHeatingEnabled);

            Assert.False(result.ActionPending);
            Assert.True(result.RdtAvailable);
        }
    }
}