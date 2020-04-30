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

            var alternateVinBaseUri = MockWeConnectConnection.BaseUri.Replace(MockWeConnectConnection.Vin, "TESTVHCLE22222222");
            mockHttp.When($"{alternateVinBaseUri}/-/cf/get-location")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""position"":
                        {
                            ""lat"": 52.433921,
                            ""lng"": 10.7957444
                        }
                    }");

            return mockHttp;
        }
    }
}