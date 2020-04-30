using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockVehicleDetails
    {
        public static MockHttpMessageHandler AddVehicleDetails(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/vehicle-info/get-vehicle-details")
                    .Respond("application/json",
                    @"{
                        ""vehicleDetails"":
                        {
                            ""lastConnectionTimeStamp"":
                            [
                                ""28-04-2020"",
                                ""14:39""
                            ],
                            ""distanceCovered"": ""64.803"",
                            ""range"": ""41"",
                            ""serviceInspectionData"": ""225 Day(s) / 25.400 km"",
                            ""oilInspectionData"": """",
                            ""showOil"": false,
                            ""showService"": true,
                            ""flightMode"": false
                        },
                        ""errorCode"": ""0""
                    }");

            return mockHttp;
        }
    }
}