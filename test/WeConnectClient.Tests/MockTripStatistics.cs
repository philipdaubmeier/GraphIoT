using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockTripStatistics
    {
        public static MockHttpMessageHandler AddTripStatistics(this MockHttpMessageHandler mockHttp)
        {
            var statisticsTypes = new[] { "shortTerm", "longTerm", "cyclic" };

            foreach (var statisticsType in statisticsTypes)
            {
                mockHttp.When($"{MockWeConnectConnection.VcfBaseUri}/tripdata/{statisticsType}/last")
                        .Respond("application/json",
                        @"{
                            ""data"":
                            {
                                ""tripEndTimestamp"": ""2021-01-01T12:00:00Z"",
                                ""id"": ""34563456"",
                                ""tripType"": """ + statisticsType + @""",
                                ""vehicleType"": ""electric"",
                                ""mileage_km"": 34,
                                ""startMileage_km"": 1200,
                                ""overallMileage_km"": 1234,
                                ""travelTime"": 60,
                                ""averageFuelConsumption"": null,
                                ""averageElectricConsumption"": 15,
                                ""averageGasConsumption"": null,
                                ""averageAuxConsumption"": null,
                                ""averageRecuperation"": null,
                                ""averageSpeed_kmph"": 50
                            }
                        }");

                mockHttp.When($"{MockWeConnectConnection.VcfBaseUri}/tripdata/{statisticsType}")
                        .Respond("application/json",
                        @"{
                            ""data"":
                            [
                                {
                                    ""tripEndTimestamp"": ""2021-01-01T12:00:00Z"",
                                    ""id"": ""34563456"",
                                    ""tripType"": """ + statisticsType + @""",
                                    ""vehicleType"": ""electric"",
                                    ""mileage_km"": 34,
                                    ""startMileage_km"": 1200,
                                    ""overallMileage_km"": 1234,
                                    ""travelTime"": 60,
                                    ""averageFuelConsumption"": null,
                                    ""averageElectricConsumption"": 15,
                                    ""averageGasConsumption"": null,
                                    ""averageAuxConsumption"": null,
                                    ""averageRecuperation"": null,
                                    ""averageSpeed_kmph"": 50
                                },
                                {
                                    ""tripEndTimestamp"": ""2021-01-01T06:00:00Z"",
                                    ""id"": ""12341234"",
                                    ""tripType"": """ + statisticsType + @""",
                                    ""vehicleType"": ""electric"",
                                    ""mileage_km"": 100,
                                    ""startMileage_km"": 1100,
                                    ""overallMileage_km"": 1200,
                                    ""travelTime"": 60,
                                    ""averageFuelConsumption"": null,
                                    ""averageElectricConsumption"": 18,
                                    ""averageGasConsumption"": null,
                                    ""averageAuxConsumption"": null,
                                    ""averageRecuperation"": null,
                                    ""averageSpeed_kmph"": 50
                                }
                            ]
                        }");
            }

            return mockHttp;
        }
    }
}