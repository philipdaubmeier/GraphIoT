using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockCarfinder
    {
        public static MockHttpMessageHandler AddCarfinder(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/cf/get-location")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""position"":
                        {
                            ""lat"": 37.377166,
                            ""lng"": -122.086966
                        }
                    }");

            return mockHttp;
        }
    }
}