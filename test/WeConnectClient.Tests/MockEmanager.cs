using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockEmanager
    {
        public static MockHttpMessageHandler AddEmanager(this MockHttpMessageHandler mockHttp)
        {
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

            return mockHttp;
        }
    }
}