using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockWarningLights
    {
        public static MockHttpMessageHandler AddWarningLights(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.VcfBaseUri}/warninglights/last")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        {
                            ""carCapturedTimestamp"": ""2021-01-01T12:00:00.000Z"",
                            ""mileage_km"": 12345,
                            ""warningLights"": [
							    {
                                    ""text"": ""Engine warning lamp on"",
                                    ""category"": ""OTHER"",
                                    ""priority"": ""100"",
                                    ""icon"": ""data:image/png;base64,Zm9vYmFy"",
                                    ""iconName"": ""dummy"",
                                    ""messageId"": ""warning.msgABCD"",
                                    ""notificationId"": 123,
                                    ""serviceLead"": true,
                                    ""customerRelevance"": true,
                                    ""timeOfOccurrence"": ""2021-01-01T12:00:00Z"",
                                    ""fieldActionCode"": ""10A1"",
                                    ""fieldActionCriteria"": ""01""
                                }
							]
                        }
                    }");

            mockHttp.When($"{MockWeConnectConnection.VcfBaseUri}/warninglights")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        [
                            {
                                ""id"": 222222,
                                ""carCapturedTimestamp"": ""2021-01-01T12:00:00.000Z"",
                                ""mileage_km"": 12345,
                                ""warningLights"": [
							        {
                                        ""text"": ""Engine warning lamp on"",
                                        ""category"": ""OTHER"",
                                        ""priority"": ""100"",
                                        ""icon"": ""data:image/png;base64,Zm9vYmFy"",
                                        ""iconName"": ""dummy"",
                                        ""messageId"": ""warning.msgABCD"",
                                        ""notificationId"": 123,
                                        ""serviceLead"": true,
                                        ""customerRelevance"": true,
                                        ""timeOfOccurrence"": ""2021-01-01T12:00:00Z"",
                                        ""fieldActionCode"": ""10A1"",
                                        ""fieldActionCriteria"": ""01""
                                    }
                                },
                            {
                                ""id"": 111111,
                                ""carCapturedTimestamp"": ""2021-01-01T06:00:00.000Z"",
                                ""mileage_km"": 12300
                            }
                        ]
                    }");

            return mockHttp;
        }
    }
}