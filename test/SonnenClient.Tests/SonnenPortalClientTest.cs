using RichardSzalay.MockHttp;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.SonnenClient.Tests
{
    public class SonnenPortalClientTest
    {
        [Fact]
        public async Task TestGetUserSites()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}users/me")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        {
                            ""id"": ""01da887a-4898-46e1-ab49-f45fc7dce4c2"",
                            ""type"": ""users"",
                            ""attributes"":
                            {
                                ""academic_title"": null,
                                ""customer_number"": ""US11122233"",
                                ""first_name"": ""John"",
                                ""last_name"": ""Doe"",
                                ""description"": null,
                                ""email"": ""john@doe.com"",
                                ""phone"": null,
                                ""mobile"": null,
                                ""street"": ""Long test street"",
                                ""postal_code"": ""12345"",
                                ""city"": ""Unittest Town"",
                                ""state"": """",
                                ""country_code"": ""US"",
                                ""latitude"": 37.377166,
                                ""longitude"": -122.086966,
                                ""language"": ""en"",
                                ""newsletter"": true,
                                ""time_zone"": ""America/New_York"",
                                ""privacy_policy"": ""EN-us-4.0"",
                                ""terms_of_service"": ""EN-us-1.0"",
                                ""service_partners_data_access"": true,
                                ""salesforce_user_id"": ""0012340000ABAABBAB"",
                                ""salesforce_contact_id"": ""00123400012ABABAAB"",
                                ""roles"": []
                            },
                            ""relationships"":
                            {
                                ""sites"":
                                {
                                    ""data"":
                                    [
                                        {
                                            ""type"": ""sites"",
                                            ""id"": """ + MockSonnenConnection.SiteId + @"""
                                        }
                                    ]
                                },
                                ""default-site"":
                                {
                                    ""data"":
                                    {
                                        ""type"": ""sites"",
                                        ""id"": """ + MockSonnenConnection.SiteId + @"""
                                    }
                                }
                            }
                        }
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetUserSites();

            Assert.Single(result.SiteIds);
            Assert.Equal(MockSonnenConnection.SiteId, result.SiteIds.First());
            Assert.Equal(MockSonnenConnection.SiteId, result.DefaultSiteId);

            Assert.Null(result.User.AcademicTitle);
            Assert.Equal("US11122233", result.User.CustomerNumber);
            Assert.Equal("John", result.User.FirstName);
            Assert.Equal("Doe", result.User.LastName);
            Assert.Null(result.User.Description);
            Assert.Equal("john@doe.com", result.User.Email);
            Assert.Null(result.User.Phone);
            Assert.Null(result.User.Mobile);
            Assert.Equal("Long test street", result.User.Street);
            Assert.Equal("12345", result.User.PostalCode);
            Assert.Equal("Unittest Town", result.User.City);
            Assert.Equal("", result.User.State);
            Assert.Equal("US", result.User.CountryCode);
            Assert.Equal(37.377166, result.User.Latitude);
            Assert.Equal(-122.086966, result.User.Longitude);
            Assert.Equal("en", result.User.Language);
            Assert.True(result.User.Newsletter);
            Assert.Equal("America/New_York", result.User.TimeZone);
            Assert.Equal("EN-us-4.0", result.User.PrivacyPolicy);
            Assert.Equal("EN-us-1.0", result.User.TermsOfService);
            Assert.True(result.User.ServicePartnersDataAccess);
            Assert.Equal("0012340000ABAABBAB", result.User.SalesforceUserId);
            Assert.Equal("00123400012ABABAAB", result.User.SalesforceContactId);
        }

        [Fact]
        public async Task TestGetBatterySystems()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}sites/{MockSonnenConnection.SiteId}/battery-systems")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        [
                            {
                                ""id"": ""ccddeeff-4f4c-4f2e-b991-a792cf020245"",
                                ""type"": ""battery-systems"",
                                ""attributes"":
                                {
                                    ""serial_number"": ""123456"",
                                    ""product_name"": ""eco 8.0"",
                                    ""installer_name"": ""Installer XY LLC"",
                                    ""installer_street"": ""Solar Street 12"",
                                    ""installer_postal_code"": ""76543"",
                                    ""installer_city"": ""Installer Town"",
                                    ""installer_state"": """",
                                    ""installer_country"": ""EN"",
                                    ""installer_phone"": ""0055512341234"",
                                    ""installer_email"": ""my@installer.com"",
                                    ""installer_account_name"": ""Installer XY"",
                                    ""installation_date"": ""2020-01-01"",
                                    ""installation_street"": ""My Street 30"",
                                    ""installation_postal_code"": ""65432"",
                                    ""installation_city"": ""My Town"",
                                    ""installation_state"": """",
                                    ""installation_country_code"": ""US"",
                                    ""time_zone"": ""America/New_York"",
                                    ""battery_capacity"": 10000,
                                    ""battery_modules"": 4,
                                    ""battery_inverter_design_power"": 3300,
                                    ""controller_type"": ""spree"",
                                    ""hardware_version"": null,
                                    ""software_version"": ""1.3.0.538728"",
                                    ""battery_charge_cycles"": 0,
                                    ""backup_power_buffer"": 0,
                                    ""backup_device_type"": ""none"",
                                    ""backup_nominal_power"": 1300,
                                    ""last_power_outage_at"": ""2020-01-02T08:06:51+02:00"",
                                    ""last_measurement_at"": ""2020-04-01T21:29:52.000+02:00"",
                                    ""cell_type"": ""sonnenModule 2500"",
                                    ""display"": false,
                                    ""article_number"": ""45744"",
                                    ""color"": ""White"",
                                    ""warranty_period"": null,
                                    ""pv_peak_power"": 9920,
                                    ""pv_grid_feed_in_limit"": 70,
                                    ""heater_connection_status"": null,
                                    ""heater_max_temperature"": null,
                                    ""smart_sockets_enabled"": false,
                                    ""online"": true
                                },
                                ""links"":
                                {
                                    ""self"": ""https://my-api.sonnen.de/v1/battery-systems/ccddeeff-4f4c-4f2e-b991-a792cf020245""
                                }
                            }
                        ]
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetBatterySystems(MockSonnenConnection.SiteId);

            Assert.Single(result);

            Assert.Equal("123456", result.First().SerialNumber);
            Assert.Equal("eco 8.0", result.First().ProductName);
            Assert.Equal("Installer XY LLC", result.First().InstallerName);
            Assert.Equal("Solar Street 12", result.First().InstallerStreet);
            Assert.Equal("76543", result.First().InstallerPostalCode);
            Assert.Equal("Installer Town", result.First().InstallerCity);
            Assert.Equal("", result.First().InstallerState);
            Assert.Equal("EN", result.First().InstallerCountry);
            Assert.Equal("0055512341234", result.First().InstallerPhone);
            Assert.Equal("my@installer.com", result.First().InstallerEmail);
            Assert.Equal("Installer XY", result.First().InstallerAccountName);
            Assert.Equal("2020-01-01", result.First().InstallationDate);
            Assert.Equal("My Street 30", result.First().InstallationStreet);
            Assert.Equal("65432", result.First().InstallationPostalCode);
            Assert.Equal("My Town", result.First().InstallationCity);
            Assert.Equal("", result.First().InstallationState);
            Assert.Equal("US", result.First().InstallationCountryCode);
            Assert.Equal("America/New_York", result.First().TimeZone);
            Assert.Equal(10000, result.First().BatteryCapacity);
            Assert.Equal(4, result.First().BatteryModules);
            Assert.Equal(3300, result.First().BatteryInverterDesignPower);
            Assert.Equal("spree", result.First().ControllerType);
            Assert.Null(result.First().HardwareVersion);
            Assert.Equal("1.3.0.538728", result.First().SoftwareVersion);
            Assert.Equal(0, result.First().BatteryChargeCycles);
            Assert.Equal(0, result.First().BackupPowerBuffer);
            Assert.Equal("none", result.First().BackupDeviceType);
            Assert.Equal(1300, result.First().BackupNominalPower);
            Assert.Equal(new DateTime(2020, 1, 2, 6, 6, 51, DateTimeKind.Utc), result.First().LastPowerOutageAt!.Value.ToUniversalTime());
            Assert.Equal(new DateTime(2020, 4, 1, 19, 29, 52, DateTimeKind.Utc), result.First().LastMeasurementAt!.Value.ToUniversalTime());
            Assert.Equal("sonnenModule 2500", result.First().CellType);
            Assert.False(result.First().Display);
            Assert.Equal("45744", result.First().ArticleNumber);
            Assert.Equal("White", result.First().Color);
            Assert.Null(result.First().WarrantyPeriod);
            Assert.Equal(9920, result.First().PvPeakPower);
            Assert.Equal(70, result.First().PvGridFeedInLimit);
            Assert.Null(result.First().HeaterConnectionStatus);
            Assert.Null(result.First().HeaterMaxTemperature);
            Assert.False(result.First().SmartSocketsEnabled);
            Assert.True(result.First().Online);
        }

        [Fact]
        public async Task TestGetStatistics()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}sites/{MockSonnenConnection.SiteId}/statistics")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        {
                            ""id"": ""ccddeeff-4f4c-4f2e-b991-a792cf020245"",
                            ""type"": ""statistics"",
                            ""attributes"":
                            {
                                ""start"": ""2020-04-01T00:00:00.000+02:00"",
                                ""end"": ""2020-04-01T01:59:59.000+02:00"",
                                ""resolution"": ""1-hour"",
                                ""measurement_method"": ""battery"",
                                ""consumed_energy"": [534],
                                ""direct_usage_energy"": [13],
                                ""produced_energy"": [34],
                                ""battery_discharged_energy"": [499],
                                ""battery_charged_energy"": [45],
                                ""battery_charged_from_sun_energy"": [56],
                                ""battery_charged_from_grid_energy"": [67],
                                ""grid_purchase_energy"": [34],
                                ""grid_feedin_energy"": [78]
                            }
                        }
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetStatistics(MockSonnenConnection.SiteId, DateTime.MinValue, DateTime.MinValue);

            Assert.NotNull(result);
            Assert.Equal(new DateTime(2020, 3, 31, 22, 00, 0, DateTimeKind.Utc), result.Start.ToUniversalTime());
            Assert.Equal(new DateTime(2020, 3, 31, 23, 59, 59, DateTimeKind.Utc), result.End.ToUniversalTime());
            Assert.Equal(new TimeSpan(1, 0, 0), result.Resolution);
            Assert.Equal("battery", result.MeasurementMethod);
            Assert.Equal(534, result.ConsumedEnergy.First());
            Assert.Equal(13, result.DirectUsageEnergy.First());
            Assert.Equal(34, result.ProducedEnergy.First());
            Assert.Equal(499, result.BatteryDischargedEnergy.First());
            Assert.Equal(45, result.BatteryChargedEnergy.First());
            Assert.Equal(56, result.BatteryChargedFromSunEnergy.First());
            Assert.Equal(67, result.BatteryChargedFromGridEnergy.First());
            Assert.Equal(34, result.GridPurchaseEnergy.First());
            Assert.Equal(78, result.GridFeedinEnergy.First());
        }

        [Fact]
        public async Task TestGetEnergyMeasurements()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}sites/{MockSonnenConnection.SiteId}/measurements")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        {
                            ""id"": ""ccddeeff-4f4c-4f2e-b991-a792cf020245"",
                            ""type"": ""measurements"",
                            ""attributes"":
                            {
                                ""start"": ""2020-04-01T00:32:00.000+02:00"",
                                ""end"": ""2020-04-01T01:32:00.000+02:00"",
                                ""resolution"": ""1m"",
                                ""measurement_method"": ""battery"",
                                ""production_power"": [9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                ""consumption_power"": [534,526,540,559,559,530,645,665,711,705,711,714,714,715,708,706,710,717,718,706,699,700,637,633,618,632,633,621,625,632,612,615,620,612,594,570,550,570,574,576,577,587,581,577,587,581,593,587,593,589,594,587,586,582,582,591,595,592,592,594],
                                ""direct_usage_power"": [7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                ""battery_charging"": [8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                ""battery_discharging"": [511,490,506,524,525,490,605,623,676,667,678,678,686,678,674,679,680,681,685,680,668,667,603,597,582,598,599,586,593,594,585,580,583,581,562,541,522,537,547,543,543,552,549,546,554,550,562,554,560,557,558,554,553,551,541,566,565,558,557,558],
                                ""grid_feedin"": [13,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
                                ""grid_purchase"": [24,36,34,35,34,40,40,42,35,38,33,35,28,38,34,27,30,36,33,26,30,33,34,35,35,34,34,35,32,38,27,35,37,31,32,28,28,32,27,32,34,34,32,30,32,31,30,32,33,32,35,32,33,31,41,25,30,33,34,35],
                                ""battery_usoc"": [91,91,91,91,91,90.03,90,90,90,90,90,90,90,89.03,89,89,89,89,89,89,88.62,88,88,88,88,88,88,88,88,87.03,87,87,87,87,87,87,87,87,86.12,86,86,86,86,86,86,86,86,85.45,85,85,85,85,85,85,85,85,84.78,84.78,84.78,84.78]
                            }
                        }
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetEnergyMeasurements(MockSonnenConnection.SiteId, DateTime.MinValue, DateTime.MinValue);

            Assert.NotNull(result);
            Assert.Equal(new DateTime(2020, 3, 31, 22, 32, 0, DateTimeKind.Utc), result.Start.ToUniversalTime());
            Assert.Equal(new DateTime(2020, 3, 31, 23, 32, 0, DateTimeKind.Utc), result.End.ToUniversalTime());
            Assert.Equal(new TimeSpan(0, 1, 0), result.Resolution);
            Assert.Equal("battery", result.MeasurementMethod);
            Assert.Equal(9, result.ProductionPower.First());
            Assert.Equal(534, result.ConsumptionPower.First());
            Assert.Equal(7, result.DirectUsagePower.First());
            Assert.Equal(8, result.BatteryCharging.First());
            Assert.Equal(511, result.BatteryDischarging.First());
            Assert.Equal(13, result.GridFeedin.First());
            Assert.Equal(24, result.GridPurchase.First());
            Assert.Equal(91, result.BatteryUsoc.First());
        }

        [Fact]
        public async Task TestGetSiteChargers()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}sites/{MockSonnenConnection.SiteId}/chargers")
                    .Respond("application/json",
                    @"{
                        ""data"": [
                            {
                                ""id"": """ + MockSonnenConnection.ChargerId + @""",
                                ""type"": ""chargers"",
                                ""attributes"": {
                                    ""serial_number"": ""12345678"",
                                    ""vendor"": ""Etrel"",
                                    ""model"": ""G-HABCDEFGH-A00"",
                                    ""charge_point_id"": ""19998877"",
                                    ""default_charging_mode"": ""SMART"",
                                    ""default_departure_time"": ""06:00:00"",
                                    ""charging_authorization_type"": ""plug_and_charge"",
                                    ""charging_unit_type"": ""KW"",
                                    ""smartmode_paused"": true,
                                    ""max_current_model"": null,
                                    ""max_current_config"": null,
                                    ""last_seen_at"": null,
                                    ""websocket_token"": ""eyJh1234123412341234.eyJp1234123412341234123412341234123412341234.baPtf-eR1n6OyvXSpEbP8Q""
                                },
                                ""relationships"": {
                                    ""cars"": {
                                        ""links"": {
                                            ""related"": ""https://my-api.sonnen.de/v1/chargers/" + MockSonnenConnection.ChargerId + @"/cars""
                                        }
                                    }
                                },
                                ""links"": {
                                    ""self"": ""https://my-api.sonnen.de/v1/chargers/" + MockSonnenConnection.ChargerId + @"""
                                }
                            }
                        ]
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetSiteChargers(MockSonnenConnection.SiteId);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("12345678", result[0].SerialNumber);
            Assert.Equal("Etrel", result[0].Vendor);
            Assert.Equal("G-HABCDEFGH-A00", result[0].Model);
            Assert.Equal("19998877", result[0].ChargePointId);
            Assert.Equal("SMART", result[0].DefaultChargingMode);
            Assert.Equal("06:00:00", result[0].DefaultDepartureTime);
            Assert.Equal("plug_and_charge", result[0].ChargingAuthorizationType);
            Assert.Equal("KW", result[0].ChargingUnitType);
            Assert.True(result[0].SmartmodePaused);
            Assert.Null(result[0].LastSeenAt);
            Assert.Equal("eyJh1234123412341234.eyJp1234123412341234123412341234123412341234.baPtf-eR1n6OyvXSpEbP8Q", result[0].WebsocketToken);
        }

        [Fact]
        public async Task TestGetChargerCars()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}chargers/{MockSonnenConnection.ChargerId}/cars")
                    .Respond("application/json",
                    @"{
                        ""data"": [
                            {
                                ""id"": ""12car123-1234-3456-1234-986765432111"",
                                ""type"": ""cars"",
                                ""attributes"": {
                                    ""consumption_kwh_100km"": 15.0,
                                    ""manufacturer"": ""VW"",
                                    ""model"": ""e-Golf"",
                                    ""active"": true,
                                    ""capacity_kwh"": 35.0,
                                    ""charged_energy"": null,
                                    ""charged_km"": null,
                                    ""created_at"": ""2020-01-01T01:02:03.000Z""
                                },
                                ""relationships"": {
                                    ""charger"": {
                                        ""links"": {
                                            ""related"": ""https://my-api.sonnen.de/v1/chargers/" + MockSonnenConnection.ChargerId + @"""
                                        },
                                        ""data"": {
                                            ""type"": ""chargers"",
                                            ""id"": """ + MockSonnenConnection.ChargerId + @"""
                                        }
                                    },
                                    ""car-model"": {
                                        ""links"": {
                                            ""related"": ""https://my-api.sonnen.de/v1/car-models/carmodel-1234-1234-1234-999887776655""
                                        },
                                        ""data"": {
                                            ""type"": ""car-models"",
                                            ""id"": ""carmodel-1234-1234-1234-999887776655""
                                        }
                                    }
                                },
                                ""links"": {
                                    ""self"": ""https://my-api.sonnen.de/v1/cars/12car123-1234-3456-1234-986765432111""
                                }
                            }
                        ],
                        ""meta"": {
                            ""total_resource_count"": 1
                        },
                        ""links"": {
                            ""first"": ""https://my-api.sonnen.de/v1/chargers/" + MockSonnenConnection.ChargerId + @"/cars?page[limit]=20"",
                            ""last"": ""https://my-api.sonnen.de/v1/chargers/" + MockSonnenConnection.ChargerId + @"/cars?page[limit]=20""
                        }
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetChargerCars(MockSonnenConnection.ChargerId);

            Assert.Equal(15, result[0].ConsumptionKwh100km);
            Assert.Equal("VW", result[0].Manufacturer);
            Assert.Equal("e-Golf", result[0].Model);
            Assert.True(result[0].Active);
            Assert.Equal(35.0, result[0].CapacityKwh);
            Assert.Null(result[0].ChargedEnergy);
            Assert.Null(result[0].ChargedKm);
            Assert.Equal(DateTime.Parse("2020-01-01T01:02:03.000Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), result[0].CreatedAt);
        }

        [Fact]
        public async Task TestGetChargerLiveState()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}chargers/{MockSonnenConnection.ChargerId}/live-state")
                    .Respond("application/json",
                    @"{
                        ""data"": {
                            ""id"": """ + MockSonnenConnection.ChargerId + @""",
                            ""type"": ""charger-live-states"",
                            ""attributes"": {
                                ""measured_at"": ""2020-05-23T01:02:03.000Z"",
                                ""active_power"": 0.0,
                                ""current"": 0.032,
                                ""charge_speed_kmh"": 0.0,
                                ""car"": ""CONNECTED"",
                                ""status"": ""CHARGING"",
                                ""max_charge_current"": 7.2,
                                ""smartmode"": true,
                                ""departure_at"": ""2020-05-23T12:00:00.000Z"",
                                ""transaction_charged_km"": 17.1,
                                ""total_charged_km"": 82.2,
                                ""total_charged_energy"": 1234.56599999999,
                                ""charging_behaviour"": ""FIRST_SCHEDULE"",
                                ""charging_authorization_required"": false
                            }
                        }
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetChargerLiveState(MockSonnenConnection.ChargerId);

            Assert.Equal(DateTime.Parse("2020-05-23T01:02:03.000Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), result.MeasuredAt);
            Assert.Equal(0.0, result.ActivePower);
            Assert.Equal(0.032, result.Current);
            Assert.Equal(0.0, result.ChargeSpeedKmh);
            Assert.Equal("CONNECTED", result.Car);
            Assert.Equal("CHARGING", result.Status);
            Assert.Equal(7.2, result.MaxChargeCurrent);
            Assert.True(result.Smartmode);
            Assert.Equal(DateTime.Parse("2020-05-23T12:00:00.000Z", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), result.DepartureAt);
            Assert.Equal(17.1, result.TransactionChargedKm);
            Assert.Equal(82.2, result.TotalChargedKm);
            Assert.Equal(1234.56599999999, result.TotalChargedEnergy);
            Assert.Equal("FIRST_SCHEDULE", result.ChargingBehaviour);
            Assert.False(result.ChargingAuthorizationRequired);
        }
    }
}