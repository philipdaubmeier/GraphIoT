using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockFuelStatus
    {
        public static MockHttpMessageHandler AddFuelStatus(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.VcfBaseUri}/fuel/status")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        [
                            {
                                ""id"": ""primaryEngine"",
                                ""carCapturedTimestamp"": ""2021-01-01T12:00:00Z"",
                                ""properties"":
                                [
                                    {
                                        ""name"": ""engineType"",
                                        ""value"": ""electric""
                                    },
                                    {
                                        ""name"": ""remainingRange_km"",
                                        ""value"": ""300""
                                    },
                                    {
                                        ""name"": ""currentSOC_pct"",
                                        ""value"": ""98""
                                    }
                                ]
                            }
                        ]
                    }");

            return mockHttp;
        }
    }
}