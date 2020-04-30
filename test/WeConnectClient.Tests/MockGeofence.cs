using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockGeofence
    {
        public static MockHttpMessageHandler AddGeofence(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/geofence/get-fences")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""geoFenceResponse"":
                        {
                            ""geoFenceList"":
                            [
                                {
                                    ""active"": false,
                                    ""id"": ""123456"",
                                    ""definitionName"": ""1607-1601 Miramonte Ave, Mountain View, CA 94040, USA"",
                                    ""zoneType"": ""RED"",
                                    ""shapeType"": ""ELLIPSE"",
                                    ""latitude"": 37.377166,
                                    ""longitude"": -122.086966,
                                    ""rotationAngle"": 0,
                                    ""rectHeight"": 0,
                                    ""rectWidth"": 0,
                                    ""ellipseFirstRadius"": 375,
                                    ""ellipseSecondRadius"": 375,
                                    ""updated"": false,
                                    ""schedule"":
                                    {
                                        ""type"": 2,
                                        ""start"": {""hours"":15,""minutes"":0},
                                        ""end"": {""hours"":20,""minutes"":0},
                                        ""index"": null,
                                        ""daypicker"": [""Y"",""Y"",""Y"",""Y"",""Y"",""N"",""N""],
                                        ""startDateActive"": ""28.04.2020"",
                                        ""endDateActive"": ""28.04.2020""
                                    },
                                    ""startDateActive"": ""28.04.2020"",
                                    ""timeRangeActive"": ""15:00 - 20:00""
                                }
                            ],
                            ""maxNumberOfGeos"": 10,
                            ""maxNumberOfActiveGeos"": 4,
                            ""status"": ""ACKNOWLEDGED"",
                            ""termAndConditionsURL"": ""/portal/web/de/content/-/content/legal/carnet-terms-and-conditions""
                        }
                    }");

            return mockHttp;
        }
    }
}