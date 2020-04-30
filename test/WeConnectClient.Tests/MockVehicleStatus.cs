using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockVehicleStatus
    {
        public static MockHttpMessageHandler AddVehicleStatus(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/vsr/get-vsr")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""vehicleStatusData"":
                        {
                            ""windowStatusSupported"": true,
                            ""carRenderData"":
                            {
                                ""parkingLights"": 2,
                                ""hood"": 3,
                                ""doors"":
                                {
                                    ""left_front"": 3,
                                    ""right_front"": 3,
                                    ""left_back"": 3,
                                    ""right_back"": 3,
                                    ""trunk"": 3,
                                    ""number_of_doors"": 4
                                },
                                ""windows"":
                                {
                                    ""left_front"": 3,
                                    ""right_front"": 3,
                                    ""left_back"": 3,
                                    ""right_back"": 3
                                },
                                ""sunroof"": 3
                            },
                            ""lockData"":
                            {
                                ""left_front"": 2,
                                ""right_front"": 2,
                                ""left_back"": 2,
                                ""right_back"": 2,
                                ""trunk"": 2
                            },
                            ""headerData"": null,
                            ""requestStatus"": null,
                            ""lockDisabled"": false,
                            ""unlockDisabled"": false,
                            ""rluDisabled"": true,
                            ""hideCngFuelLevel"": false,
                            ""totalRange"": 41,
                            ""primaryEngineRange"": 41,
                            ""fuelRange"": null,
                            ""cngRange"": null,
                            ""batteryRange"": 41,
                            ""fuelLevel"": null,
                            ""cngFuelLevel"": null,
                            ""batteryLevel"": 100,
                            ""sliceRootPath"": ""https://www.portal.volkswagen-we.com/static/slices/phev_golf/phev_golf""
                        }
                    }");

            return mockHttp;
        }
    }
}