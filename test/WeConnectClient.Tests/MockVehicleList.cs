using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockVehicleList
    {
        public static MockHttpMessageHandler AddVehicleList(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/mainnavigation/get-fully-loaded-cars")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""fullyLoadedVehiclesResponse"":
                        {
                            ""completeVehicles"": [],
                            ""vehiclesNotFullyLoaded"":
                            [
                                " + GetVehicleDetailJson("WVWZZZABCD1234567") + @"
                            ],
                            ""status"": ""VALID"",
                            ""currentVehicleValid"": true
                        }
                    }");

            return mockHttp;
        }

        public static MockHttpMessageHandler AddLoadCarDetails(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.BaseUri}/-/mainnavigation/load-car-details/")
                    .Respond("application/json",
                    @"{
                        ""errorCode"": ""0"",
                        ""completeVehicleJson"": " + GetVehicleDetailJson("WVWZZZABCD1234567") + @"
                    }");

            return mockHttp;
        }

        private static string GetVehicleDetailJson(string vin)
        {
            return @"{
                        ""vin"": """ + vin + @""",
                        ""name"": ""My Car"",
                        ""expired"": false,
                        ""model"": null,
                        ""modelCode"": null,
                        ""modelYear"": null,
                        ""imageUrl"": null,
                        ""vehicleSpecificFallbackImageUrl"": null,
                        ""modelSpecificFallbackImageUrl"": null,
                        ""defaultImageUrl"": ""/portal/delegate/vehicle-image/WVWZZZABCD1234567"",
                        ""vehicleBrand"": ""v"",
                        ""enrollmentDate"": ""20200101"",
                        ""deviceOCU1"": null,
                        ""deviceOCU2"": null,
                        ""deviceMIB"": null,
                        ""engineTypeCombustian"": false,
                        ""engineTypeHybridOCU1"": true,
                        ""engineTypeHybridOCU2"": false,
                        ""engineTypeElectric"": false,
                        ""engineTypeCNG"": false,
                        ""engineTypeDefault"": false,
                        ""stpStatus"": ""UNAVAILABLE"",
                        ""windowstateSupported"": true,
                        ""dashboardUrl"": ""/portal/user/12341234-1234-1234-1234-123412341234/abcdefg_unit_test_xprofile_id_12"",
                        ""vhrRequested"": false,
                        ""vsrRequested"": false,
                        ""vhrConfigAvailable"": false,
                        ""verifiedByDealer"": false,
                        ""vhr2"": false,
                        ""roleEnabled"": true,
                        ""isEL2Vehicle"": true,
                        ""workshopMode"": false,
                        ""hiddenUserProfiles"": false,
                        ""mobileKeyActivated"": null,
                        ""enrollmentType"": ""MILEAGE"",
                        ""ocu3Low"": false,
                        ""packageServices"":
                        [
                            {
                                ""packageServiceId"": ""NET.500.010.F"",
                                ""propertyKeyReference"": ""NET.500.010.1"",
                                ""packageServiceName"": ""e-Remote"",
                                ""trackingName"": ""e-Remote"",
                                ""activationDate"": ""01-01-2020"",
                                ""expirationDate"": ""01-01-2021"",
                                ""expired"": false,
                                ""expireInAMonth"": false,
                                ""packageType"": ""er"",
                                ""enrollmentPackageType"": ""er""
                            }
                        ],
                        ""defaultCar"": true,
                        ""vwConnectPowerLayerAvailable"": false,
                        ""xprofileId"": ""abcdefg_unit_test_xprofile_id_12"",
                        ""smartCardKeyActivated"": null,
                        ""fullyEnrolled"": true,
                        ""secondaryUser"": false,
                        ""fleet"": false,
                        ""touareg"": false,
                        ""iceSupported"": false,
                        ""flightMode"": false,
                        ""esimCompatible"": false,
                        ""dkyenabled"": false,
                        ""selected"": true
                    }";
        }
    }
}