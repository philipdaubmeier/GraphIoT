using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockVehicleData
    {
        public static MockHttpMessageHandler AddVehicleData(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.GvfBaseUri}/vehicleData/de-DE/{MockWeConnectConnection.Vin}")
                    .Respond("application/json",
                    @"{
                        ""vin"": """ + MockWeConnectConnection.Vin + @""",
                        ""modelName"": ""Golf"",
                        ""exteriorColor"": ""5K""
                    }");

            mockHttp.When($"{MockWeConnectConnection.GvfBaseUri}/vehicleDetails/de-DE/{MockWeConnectConnection.Vin}")
                    .Respond("application/json",
                    @"{
                        ""engine"": ""100 kW (136 PS)"",
                        ""specifications"":
                        [
                            {
                                ""codeText"": ""Description text 1"",
                                ""origin"": """"
                            },
                            {
                                ""codeText"": ""Description text 2"",
                                ""origin"": """"
                            }
                        ],
                        ""modelYear"": ""2021"",
                        ""exteriorColorText"": ""Uranograu""
                    }");

            return mockHttp;
        }
    }
}