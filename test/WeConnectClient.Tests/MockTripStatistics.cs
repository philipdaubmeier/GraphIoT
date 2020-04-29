using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockTripStatistics
    {
        public static MockHttpMessageHandler AddTripStatistics(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/rts/get-latest-trip-statistics")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""rtsViewModel"":
                        {
                            ""daysInMonth"": 30,
                            ""firstWeekday"": 2,
                            ""month"": 4,
                            ""year"": 2020,
                            ""firstTripYear"": 2020,
                            ""tripStatistics"":
                            [
                                null, null, null, null, null, null, null, null, null, null, null, null, null, null, 
			                    null, null, null, null, null, null, null, null, null, null, null, null, null,
                                {
                                    ""aggregatedStatistics"":
                                    {
                                        ""tripId"": 123412344,
                                        ""averageElectricConsumption"": 12.8,
                                        ""averageFuelConsumption"": null,
                                        ""averageCngConsumption"": null,
                                        ""averageSpeed"": 50,
                                        ""tripDuration"": 70,
                                        ""tripLength"": 58,
                                        ""timestamp"": ""28.04.2020"",
                                        ""tripDurationFormatted"": ""1:10"",
                                        ""recuperation"": null,
                                        ""averageAuxiliaryConsumption"": null,
                                        ""totalElectricConsumption"": 12.8,
                                        ""longFormattedTimestamp"": null
                                    },
                                    ""tripStatistics"":
                                    [
                                        {
                                            ""tripId"": 123412344,
                                            ""averageElectricConsumption"": 12.8,
                                            ""averageFuelConsumption"": null,
                                            ""averageCngConsumption"": null,
                                            ""averageSpeed"": 50,
                                            ""tripDuration"": 70,
                                            ""tripLength"": 58,
                                            ""timestamp"": ""Today, 14:34"",
                                            ""tripDurationFormatted"": ""1:10"",
                                            ""recuperation"": null,
                                            ""averageAuxiliaryConsumption"": null,
                                            ""totalElectricConsumption"": null,
                                            ""longFormattedTimestamp"": ""Trip ended: Tue, 28.04.2020, 14:34""
                                        }
                                    ]
                                },
                                null, null
                            ],
                            ""longTermData"":
                            {
                                ""tripId"": 123412344,
                                ""averageElectricConsumption"": 13.1,
                                ""averageFuelConsumption"": null,
                                ""averageCngConsumption"": null,
                                ""averageSpeed"": 26,
                                ""tripDuration"": 642,
                                ""tripLength"": 275,
                                ""timestamp"": ""Heute, 14:34"",
                                ""tripDurationFormatted"": ""10:42"",
                                ""recuperation"": null,
                                ""averageAuxiliaryConsumption"": null,
                                ""totalElectricConsumption"": null,
                                ""longFormattedTimestamp"": ""Trip ended: Tue, 28.04.2020, 14:34""
                            },
                            ""cyclicData"": null,
                            ""serviceConfiguration"":
                            {
                                ""electric_consumption"": false,
                                ""triptype_short"": true,
                                ""auxiliary_consumption"": false,
                                ""fuel_overall_consumption"": false,
                                ""triptype_cyclic"": true,
                                ""electric_overall_consumption"": true,
                                ""triptype_long"": true,
                                ""cng_overall_consumption"": false,
                                ""recuperation"": false
                            },
                            ""tripFromLastRefuelAvailable"": false
                        }
                    }");

            return mockHttp;
        }
    }
}