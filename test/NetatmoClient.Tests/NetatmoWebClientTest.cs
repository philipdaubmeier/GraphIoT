using PhilipDaubmeier.NetatmoClient.Model.Core;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.NetatmoClient.Tests
{
    public class NetatmoWebClientTest
    {
        private static ModuleId _moduleId = "12:34:56:78:9a:bc";

        [Fact]
        public async Task TestGetWeatherStationMainModuleInfo()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Single(result.Devices);
            Assert.Equal("12:34:56:78:9a:bc", result.Devices[0].Id);
            Assert.Equal("My Netatmo", result.Devices[0].StationName);
            Assert.Equal("MyMainModule", result.Devices[0].ModuleName);
            Assert.Equal("NAMain", result.Devices[0].Type);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1450976717), result.Devices[0].DateSetup);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1450976717), result.Devices[0].LastSetup);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585688537), result.Devices[0].LastStatusStore);
            Assert.Equal(137, result.Devices[0].Firmware);
            Assert.Equal(62, result.Devices[0].WifiStatus);
            Assert.False(result.Devices[0].Co2Calibrating);
            Assert.Equal(new List<Measure>() { MeasureType.Temperature, MeasureType.CO2, MeasureType.Humidity, MeasureType.Noise, MeasureType.Pressure }, result.Devices[0].DataType);
        }

        [Fact]
        public async Task TestGetWeatherStationMainModulePlace()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Equal(321, result.Devices[0].Place.Altitude);
            Assert.Equal("Unittest Town", result.Devices[0].Place.City);
            Assert.Equal("EN", result.Devices[0].Place.Country);
            Assert.Equal("America/New_York", result.Devices[0].Place.Timezone);
            Assert.Equal(37.377166, result.Devices[0].Place.Location[0]);
            Assert.Equal(-122.086966, result.Devices[0].Place.Location[1]);
        }

        [Fact]
        public async Task TestGetWeatherStationMainModuleMeasurements()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585688522), result.Devices[0].DashboardData.TimeUtc);
            Assert.Equal(20.7, result.Devices[0].DashboardData.Temperature);
            Assert.Equal(1407, result.Devices[0].DashboardData.CO2);
            Assert.Equal(59, result.Devices[0].DashboardData.Humidity);
            Assert.Equal(35, result.Devices[0].DashboardData.Noise);
            Assert.Equal(1025, result.Devices[0].DashboardData.Pressure);
            Assert.Equal(980.7, result.Devices[0].DashboardData.AbsolutePressure);
            Assert.Equal(20, result.Devices[0].DashboardData.MinTemp);
            Assert.Equal(21.3, result.Devices[0].DashboardData.MaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585650280), result.Devices[0].DashboardData.DateMaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585618645), result.Devices[0].DashboardData.DateMinTemp);
            Assert.Equal("stable", result.Devices[0].DashboardData.TempTrend);
        }

        [Fact]
        public async Task TestGetWeatherStationOutdoorModuleInfo()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Equal("98:76:54:32:1a:bc", result.Devices[0].Modules[0].Id);
            Assert.Equal("My outdoor module", result.Devices[0].Modules[0].ModuleName);
            Assert.Equal("NAModule1", result.Devices[0].Modules[0].Type);
            Assert.Equal(44, result.Devices[0].Modules[0].Firmware);
            Assert.Equal(57, result.Devices[0].Modules[0].RfStatus);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1450976705), result.Devices[0].Modules[0].LastSetup);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722586), result.Devices[0].Modules[0].LastMessage);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722547), result.Devices[0].Modules[0].LastSeen);
            Assert.Equal(5376, result.Devices[0].Modules[0].BatteryVp);
            Assert.Equal(74, result.Devices[0].Modules[0].BatteryPercent);
            Assert.Equal(new List<Measure>() { MeasureType.Temperature, MeasureType.Humidity }, result.Devices[0].Modules[0].DataType);
        }

        [Fact]
        public async Task TestGetWeatherStationOutdoorModuleMeasurements()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722547), result.Devices[0].Modules[0].DashboardData.TimeUtc);
            Assert.Equal(0, result.Devices[0].Modules[0].DashboardData.Temperature);
            Assert.Equal(59, result.Devices[0].Modules[0].DashboardData.Humidity);
            Assert.Equal(-1.4, result.Devices[0].Modules[0].DashboardData.MinTemp);
            Assert.Equal(2.4, result.Devices[0].Modules[0].DashboardData.MaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585692196), result.Devices[0].Modules[0].DashboardData.DateMaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585717163), result.Devices[0].Modules[0].DashboardData.DateMinTemp);
            Assert.Equal("up", result.Devices[0].Modules[0].DashboardData.TempTrend);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.CO2);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.Noise);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.Pressure);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.AbsolutePressure);
        }

        [Fact]
        public async Task TestGetWeatherStationRainModuleInfo()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Equal("bc:de:f9:87:65:43", result.Devices[0].Modules[1].Id);
            Assert.Equal("Rain module", result.Devices[0].Modules[1].ModuleName);
            Assert.Equal("NAModule2", result.Devices[0].Modules[1].Type);
            Assert.Equal(new List<Measure>() { MeasureType.Rain }, result.Devices[0].Modules[1].DataType);
        }

        [Fact]
        public async Task TestGetWeatherStationRainModuleData()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722573), result.Devices[0].Modules[1].DashboardData.TimeUtc);
            Assert.Equal(0, result.Devices[0].Modules[1].DashboardData.Rain);
            Assert.Equal(0, result.Devices[0].Modules[1].DashboardData.SumRain1);
            Assert.Equal(0, result.Devices[0].Modules[1].DashboardData.SumRain24);
            Assert.Null(result.Devices[0].Modules[1].DashboardData.Temperature);
            Assert.Null(result.Devices[0].Modules[1].DashboardData.CO2);
            Assert.Null(result.Devices[0].Modules[1].DashboardData.Noise);
        }

        [Fact]
        public async Task TestGetWeatherStationUserSettings()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddWeatherStation()
                .ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Equal("john@doe.com", result.User.Mail);
            Assert.Equal("US", result.User.Administrative.Country);
            Assert.Equal("en-US", result.User.Administrative.RegLocale);
            Assert.Equal("en", result.User.Administrative.Lang);
            Assert.Equal(0, result.User.Administrative.Unit);
            Assert.Equal(0, result.User.Administrative.Windunit);
            Assert.Equal(0, result.User.Administrative.Pressureunit);
            Assert.Equal(0, result.User.Administrative.FeelLikeAlgo);
        }

        [Fact]
        public async Task TestGetHomeDataHomes()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddHomeData()
                .ToMockProvider());

            var result = await netatmoClient.GetHomeData();

            Assert.Single(result.Homes);
            Assert.Equal("1234abcd1234abcd1234abcd", result.Homes[0].Id);
            Assert.Equal("My Netatmo Home", result.Homes[0].Name);
            Assert.Equal("Unittest Town", result.Homes[0].Place.City);
            Assert.Equal("EN", result.Homes[0].Place.Country);
            Assert.Equal("America/New_York", result.Homes[0].Place.Timezone);
        }

        [Fact]
        public async Task TestGetHomeDataCameras()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddHomeData()
                .ToMockProvider());

            var result = await netatmoClient.GetHomeData();

            Assert.Single(result.Homes[0].Cameras);
            Assert.Equal("78:90:ab:cd:ef:01", result.Homes[0].Cameras[0].Id);
            Assert.Equal("NOC", result.Homes[0].Cameras[0].Type);
            Assert.Equal("on", result.Homes[0].Cameras[0].Status);
            Assert.Equal("https://prodvpn-eu-123.netatmo.net/restricted/10.0.0.1/abcabc123412341234123412341234ab/MTABCDEFG1234ABCDEF1234BACDEF1234ABCDEF123,,", result.Homes[0].Cameras[0].VpnUrl);
            Assert.False(result.Homes[0].Cameras[0].IsLocal);
            Assert.Equal("on", result.Homes[0].Cameras[0].SdStatus);
            Assert.Equal("on", result.Homes[0].Cameras[0].AlimStatus);
            Assert.Equal("My Presence", result.Homes[0].Cameras[0].Name);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1490468965), result.Homes[0].Cameras[0].LastSetup);
            Assert.Equal("auto", result.Homes[0].Cameras[0].LightModeStatus);
        }

        [Fact]
        public async Task TestGetHomeDataEventInfo()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddHomeData()
                .ToMockProvider());

            var result = await netatmoClient.GetHomeData();

            Assert.Single(result.Homes[0].Events);
            Assert.Equal("1234abcd1234abcd1234abcd", result.Homes[0].Events[0].Id);
            Assert.Equal("outdoor", result.Homes[0].Events[0].Type);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585576467), result.Homes[0].Events[0].Time);
            Assert.Equal("78:90:ab:cd:ef:01", result.Homes[0].Events[0].CameraId);
            Assert.Equal("78:90:ab:cd:ef:01", result.Homes[0].Events[0].DeviceId);
            Assert.Equal("1234d30c-1234-5678-9012-abc35d7a4abc", result.Homes[0].Events[0].VideoId);
            Assert.Equal("available", result.Homes[0].Events[0].VideoStatus);
            Assert.NotEmpty(result.Homes[0].Events[0].EventList);
        }

        [Fact]
        public async Task TestGetHomeDataEventListFirst()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddHomeData()
                .ToMockProvider());

            var result = await netatmoClient.GetHomeData();

            Assert.Equal("vehicle", result.Homes[0].Events[0].EventList[0].Type);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585726685), result.Homes[0].Events[0].EventList[0].Time);
            Assert.Equal(0, result.Homes[0].Events[0].EventList[0].Offset);
            Assert.Equal("1234d30c-1234-5678-9012-ff735af0923f", result.Homes[0].Events[0].EventList[0].Id);
            Assert.Equal("Vehicle detected", result.Homes[0].Events[0].EventList[0].Message);
            Assert.Equal("123e75a5980d8c6c79ef111a", result.Homes[0].Events[0].EventList[0].Snapshot.Id);
            Assert.Equal(1, result.Homes[0].Events[0].EventList[0].Snapshot.Version);
            Assert.Equal("123e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edbe5ba00e5ba", result.Homes[0].Events[0].EventList[0].Snapshot.Key);
            Assert.Equal("https://netatmocameraimage.blob.domain.com/path/b178de75e8444df3c9d762bd452bc2d625a44e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edb", result.Homes[0].Events[0].EventList[0].Snapshot.Url);
            Assert.Equal(string.Empty, result.Homes[0].Events[0].EventList[0].Snapshot.Filename);
            Assert.Equal("234e75a5980d8c6c79ef111a", result.Homes[0].Events[0].EventList[0].Vignette.Id);
            Assert.Equal(1, result.Homes[0].Events[0].EventList[0].Vignette.Version);
            Assert.Equal("234e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edbe5ba00e5ba", result.Homes[0].Events[0].EventList[0].Vignette.Key);
            Assert.Equal("https://netatmocameraimage.blob.domain.com/path/b178de75e8444df3c9d762bd452bc2d625a44e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edb", result.Homes[0].Events[0].EventList[0].Vignette.Url);
            Assert.Equal(string.Empty, result.Homes[0].Events[0].EventList[0].Vignette.Filename);
        }

        [Fact]
        public async Task TestGetHomeDataEventListSubsequent()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddHomeData()
                .ToMockProvider());

            var result = await netatmoClient.GetHomeData();

            Assert.Equal("human", result.Homes[0].Events[0].EventList[1].Type);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585576470), result.Homes[0].Events[0].EventList[1].Time);
            Assert.Equal(1, result.Homes[0].Events[0].EventList[1].Offset);
            Assert.Equal("1234d30c-1234-5678-9012-e5b34ccbd5b3", result.Homes[0].Events[0].EventList[1].Id);
            Assert.Equal("Person detected", result.Homes[0].Events[0].EventList[1].Message);
            Assert.Equal("vod/1234d30c-1234-5678-9012-abc35d7a4abc/events/1234d30c-1234-5678-9012-e5b34ccbd5b3/snapshot_1234d30c-1234-5678-9012-e5b34ccbd5b3.jpg", result.Homes[0].Events[0].EventList[1].Snapshot.Filename);
            Assert.Equal("vod/1234d30c-1234-5678-9012-abc35d7a4abc/events/1234d30c-1234-5678-9012-e5b34ccbd5b3/vignette_1234d30c-1234-5678-9012-e5b34ccbd5b3.jpg", result.Homes[0].Events[0].EventList[1].Vignette.Filename);
        }

        [Fact]
        public async Task TestGetHomeDataUser()
        {
            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddHomeData()
                .ToMockProvider());

            var result = await netatmoClient.GetHomeData();

            Assert.Equal("en-US", result.User.RegLocale);
            Assert.Equal("en", result.User.Lang);
            Assert.Equal("US", result.User.Country);
            Assert.Equal("john@doe.com", result.User.Mail);
            Assert.True(result.GlobalInfo.ShowTags);
        }

        [Fact]
        public async Task TestGetMeasureTempCo2Humidity()
        {
            var measures = new List<Measure>() { MeasureType.Temperature, MeasureType.CO2, MeasureType.Humidity };

            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddMeasurements(_moduleId, measures, 30)
                .ToMockProvider());

            var result = await netatmoClient.GetMeasure(_moduleId, _moduleId, measures);

            Assert.Equal(3, result.Count());
            Assert.Equal(30, result.First().Value.Count());
            Assert.Equal(30, result.Skip(1).First().Value.Count());
            Assert.Equal(30, result.Skip(2).First().Value.Count());

            Assert.Equal(21.3, result.First().Value.First().Value);
            Assert.Equal(524, result.Skip(1).First().Value.First().Value);
            Assert.Equal(58, result.Skip(2).First().Value.First().Value);

            Assert.Equal(21.3, result.First().Value.Skip(20).First().Value);
            Assert.Equal(524, result.Skip(1).First().Value.Skip(20).First().Value);
            Assert.Equal(58, result.Skip(2).First().Value.Skip(20).First().Value);
        }

        [Fact]
        public async Task TestGetMeasureRain()
        {
            var measures = new List<Measure>() { MeasureType.Rain };

            using var netatmoClient = new NetatmoWebClient(new MockHttpMessageHandler()
                .AddAuthMock()
                .AddMeasurements(_moduleId, measures, 5)
                .ToMockProvider());

            var result = await netatmoClient.GetMeasure(_moduleId, _moduleId, measures);

            Assert.Single(result);
            Assert.Equal(5, result.First().Value.Count());

            Assert.Equal(0, result.First().Value.First().Value);
        }
    }
}
