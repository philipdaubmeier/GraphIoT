using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockHealthReport
    {
        public static MockHttpMessageHandler AddHealthReport(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/vhr/get-latest-report")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""vehicleHealthReportList"":
                        [
                            {
                                ""creationDate"": ""23.04.2020"",
                                ""creationTime"": ""21:24"",
                                ""diagnosticMessages"": [],
                                ""hasValidData"": true,
                                ""reportId"": ""123456"",
                                ""isOlderSixMonths"": false,
                                ""mileageValue"": ""150"",
                                ""timestamp"": 1587669881536,
                                ""headerData"":
                                {
                                    ""rangeMileage"": """",
                                    ""lastRefreshTime"":
                                    [
                                        ""23.04.2020"",
                                        ""21:24""
                                    ],
                                    ""serviceOverdue"": false,
                                    ""oilOverdue"": false,
                                    ""mileage"": ""89"",
                                    ""showOil"": false,
                                    ""vhrNoDataText"": null,
                                    ""showService"": true
                                },
                                ""jobStatus"": ""ACKNOWLEDGED"",
                                ""fetchFailed"": false
                            }
                        ],
                        ""X-RateLimit-CreationTask-Reset"": ""691200"",
                        ""X-RateLimit-CreationTask-Limit"": ""30"",
                        ""X-RateLimit-CreationTask-Remaining"": ""29""
                    }");

            return mockHttp;
        }
    }
}